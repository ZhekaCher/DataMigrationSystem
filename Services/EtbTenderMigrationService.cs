using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata.EtbTender;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class EtbTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        
        public EtbTenderMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
        }
        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            
            await MigrateReferences();
            await Migrate();

            Logger.Info("End of migration");
            
            await using var webTenderContext = new WebTenderContext();
            // await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            // await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");
            
            await using var parsedEtbContext = new ParsedEtbTenderContext();
            await parsedEtbContext.Database.ExecuteSqlRawAsync("truncate table avroradata.etb_trade_tender, avroradata.etb_trade_lot, avroradata.etb_trade_lot_detalization restart identity");
        }

        private async Task Insert(EtbTenderDto dto)
        {
            // This is supposed to mean tender
            var announcement = DtoToWebAnnouncement(dto);
            try
            {
                await using var webTenderContext = new WebTenderContext();
                webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var found = webTenderContext.AdataAnnouncements.Select(x => new {x.Id, x.SourceNumber, x.SourceId})
                    .FirstOrDefault(x =>
                        x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);

                if (found != null)
                {
                    await webTenderContext.AdataAnnouncements.Upsert(announcement)
                        .On(x => new {x.SourceNumber, x.SourceId})
                        .UpdateIf((x, y) =>
                            x.StatusId != y.StatusId || x.LotsQuantity != y.LotsQuantity || x.MethodId != y.MethodId ||
                            x.TenderPriorityId != y.TenderPriorityId).RunAsync();
                    foreach (var lot in announcement.Lots)
                    {
                        lot.AnnouncementId = found.Id;
                        var foundLot = webTenderContext.AdataLots.Select(x => new {x.Id, x.SourceNumber, x.SourceId})
                            .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                        if (foundLot != null)
                        {
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .UpdateIf((x, y) =>
                                    x.StatusId != y.StatusId || x.Characteristics != y.Characteristics ||
                                    x.MethodId != y.MethodId || x.MeasureId != y.MeasureId ||
                                    x.SupplyLocation != y.SupplyLocation).RunAsync();
                            if (lot.PaymentCondition == null) continue;
                            lot.PaymentCondition.LotId = foundLot.Id;
                            await webTenderContext.PaymentConditions.Upsert(lot.PaymentCondition).On(x => x.LotId)
                                .UpdateIf((x, y) =>
                                    x.InterimPayment != y.InterimPayment || x.FinalPayment != y.FinalPayment ||
                                    x.PrepayPayment != y.PrepayPayment).RunAsync();
                        }
                        else
                        {
                            await webTenderContext.AdataLots.AddAsync(lot);
                            await webTenderContext.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    await webTenderContext.AdataAnnouncements.AddAsync(announcement);
                    await webTenderContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace + e.InnerException + e.Message);
                Console.WriteLine(e.InnerException);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }
        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsedEtbContext = new ParsedEtbTenderContext();
            
            // Gets all Tender DTO listings, tries to insert them one-by-one
            var etbFullDtos = parsedEtbContext.EtbTenders
                .AsNoTracking()
                .Include(x => x.EtbLots)
                .ThenInclude(x => x.EtbDetails);
            var tasks = new List<Task>();
            foreach (var dto in etbFullDtos)
            {
                tasks.Add(Insert(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            
            await Task.WhenAll(tasks);
        }

        private async Task MigrateReferences()
        {
            await using var webTenderContext = new WebTenderContext();
            await using var parsedEtbContext = new ParsedEtbTenderContext();
            _total = await parsedEtbContext.EtbTenders.CountAsync();
            var units = parsedEtbContext.EtbLots.Select(x=> new Measure {Name = x.MeasurementUnits}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();
            var truCodes = parsedEtbContext.EtbTenders.Select(x=> new TruCode {Code = x.IdGroupFull, Name = x.NameGroupLots}).Distinct().Where(x=>x.Name!=null);;
            await webTenderContext.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();
            var methods = parsedEtbContext.EtbTenders.Select(x=> new Method{Name = x.CompletionStrategy}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Methods.UpsertRange(methods).On(x => x.Name).NoUpdate().RunAsync();
            var statuses = parsedEtbContext.EtbTenders.Select(x => new Status{Name = x.Status}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();
            foreach (var dict in webTenderContext.Measures)
                _measures.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Statuses)
                _statuses.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Methods) 
                _methods.Add(dict.Name, dict.Id);
        }
        
        /// <summary>
        /// Transforms from Etb Tender to Web Tender (announcement) form
        /// </summary>
        /// <param name="dto"> Etb Tender DTO</param>
        /// <returns></returns>
        private AdataAnnouncement DtoToWebAnnouncement(EtbTenderDto dto)
        {
            var announcement = new AdataAnnouncement
            {
                SourceNumber = dto.IdGroupFull,
                Title =  dto.NameGroupLots,
                ApplicationStartDate =  dto.DatePublished,
                ApplicationFinishDate = dto.DateFinAccepting,
                CustomerBin = null,
                LotsAmount =  dto.TotalNoTaxes,
                LotsQuantity = dto.LotsInGroup,
                SourceId = 10,
                EmailAddress = dto.ContactMail,
                PhoneNumber = dto.ContactPhone,
                PublishDate = dto.DatePublished,
            };
            announcement.SourceLink = $"https://etbemp.kz/ru/Protocol/Get/{dto.IdGroup}?status=1";
            
            if (dto.Status != null)
            {
                if (_statuses.TryGetValue(dto.Status, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }
            if (dto.CompletionStrategy != null)
            {
                if (_methods.TryGetValue(dto.CompletionStrategy, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }
            
            // Documentation is empty, for now
            //announcement.Documentations = new List<AnnouncementDocumentation>();
            
            announcement.Lots = new List<AdataLot>();
            foreach (var dtoLot in dto.EtbLots)
            {
                var detailsRequired = dtoLot.Dkc == null
                                      || dtoLot.PaymentCondition == null
                                      || dtoLot.CompletionPeriod == null
                                      || dtoLot.UnitTotalNoTax == null
                                      || dtoLot.MeasurementUnits == null;

                var lot = new AdataLot
                {
                    SourceNumber = dto.IdGroupFull + "-" + dtoLot.IdLot.ToString(),
                    Title = dtoLot.Title,
                    SourceId = 10,
                    ApplicationStartDate = dto.DatePublished,
                    ApplicationFinishDate = dto.DateFinAccepting,
                    CustomerBin = null,
                    SupplyLocation = dtoLot.CompletionCondition,
                    TenderLocation = dto.CompanyAddress,
                    Characteristics = dtoLot.ShortDescription,
                    TotalAmount = dtoLot.GroupTotalNoTax,
                    UnitPrice = dtoLot.UnitTotalNoTax ?? 0,
                    Terms = dtoLot.CompletionPeriod,
                    TruCode = dto.IdGroupFull,
                    DeliveryConditions = dtoLot.PaymentCondition,
                };
                try
                {
                    if (dtoLot.NOrders != null) lot.Quantity = (double) dtoLot.NOrders;
                }
                catch (Exception e)
                {
                    // Nothing
                }
                lot.SourceLink = $"https://etbemp.kz/ru/Protocol/Get/{dto.IdGroup}?status=1";
                if (dto.Status != null)
                {
                    if (_statuses.TryGetValue(dto.Status, out var temp))
                    {
                        lot.StatusId = temp;
                    }
                }
                if (dto.CompletionStrategy != null)    //TODO: payment conditions -> Error reference no object
                {
                    if (_methods.TryGetValue(dto.CompletionStrategy, out var temp))
                    {
                        lot.MethodId = temp;
                    }
                }
                if (dtoLot.MeasurementUnits != null)
                {
                    if (_measures.TryGetValue(dtoLot.MeasurementUnits, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }
                announcement.Lots.Add(lot);
            }
            return announcement;
        }
    }
}