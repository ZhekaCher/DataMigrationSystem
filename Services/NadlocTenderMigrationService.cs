using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class NadlocTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        public NadlocTenderMigrationService(int numOfThreads = 10)
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
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");
            
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            await parsedAnnouncementNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_tenders, avroradata.nadloc_lots");
        }

        private async Task Insert(AnnouncementNadlocDto dto)
        {
            // await Task.Delay(50);
            var announcement = DtoToWebAnnouncement(dto);
            try
            {
                await using var webTenderContext = new WebTenderContext();
                webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var found = webTenderContext.AdataAnnouncements
                    .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
                {
                    if (announcement.StatusId != 1 && found.StatusId == 1)
                    {
                        found.StatusId=announcement.StatusId;
                        await webTenderContext.AdataLots.Where(x => x.AnnouncementId == found.Id)
                            .ForEachAsync(x => x.StatusId = announcement.StatusId);
                        await webTenderContext.SaveChangesAsync();
                    }
                    else
                    {
                        await webTenderContext.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                            .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.LotsQuantity != y.LotsQuantity || x.MethodId != y.MethodId || x.TenderPriorityId != y.TenderPriorityId).RunAsync();
                        foreach (var lot in announcement.Lots)
                        {
                            lot.AnnouncementId = found.Id;
                            var foundLot = webTenderContext.AdataLots.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                                .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                            if (foundLot != null)
                            {
                                await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                    .UpdateIf((x, y) =>
                                        x.StatusId != y.StatusId || x.Characteristics != y.Characteristics ||
                                        x.MethodId != y.MethodId || x.MeasureId != y.MeasureId ||
                                        x.SupplyLocation != y.SupplyLocation).RunAsync();
                            }
                            else
                            {
                                await webTenderContext.AdataLots.AddAsync(lot);
                                await webTenderContext.SaveChangesAsync();
                            }
                        }
                        // webTenderContext.AdataLots.RemoveRange(
                        //     webTenderContext.AdataLots.Where(x => x.AnnouncementId == found.Id));
                        // await webTenderContext.SaveChangesAsync();
                        // announcement.Lots.ForEach(x => x.AnnouncementId = found.Id);
                        // await webTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                        // await webTenderContext.SaveChangesAsync();
                        // await webTenderContext.AdataAnnouncements.Upsert(announcement)
                        //     .On(x => new {x.SourceNumber, x.SourceId})
                        //     .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.LotsQuantity != y.LotsQuantity || x.MethodId != y.MethodId || x.TenderPriorityId != y.TenderPriorityId).RunAsync();
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
                Logger.Error(e);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }
        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            var nadlocDtos = parsedAnnouncementNadlocContext.AnnouncementNadlocDtos
                .AsNoTracking()
                .Include(x=>x.Lots);
            var tasks = new List<Task>();
            foreach (var dto in nadlocDtos)
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
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            _total = await parsedAnnouncementNadlocContext.AnnouncementNadlocDtos.CountAsync();
            var units = parsedAnnouncementNadlocContext.LotNadlocDtos.Select(x=>new Measure {Name = x.Unit}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();
            var truCodes = parsedAnnouncementNadlocContext.LotNadlocDtos.Select(x=> new TruCode {Code = x.ScpCode, Name = x.ScpDescription}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();
            foreach (var dict in webTenderContext.Measures)
                _measures.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Statuses)
                _statuses.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Methods)
                _methods.Add(dict.Name, dict.Id);
        }
        private AdataAnnouncement DtoToWebAnnouncement(AnnouncementNadlocDto dto)
        {
            // await using var webTenderContext = new AdataTenderContext();
            var announcement = new AdataAnnouncement
            {
                SourceNumber =  dto.FullId,
                Title =  dto.Name,
                ApplicationStartDate =  dto.DateStart,
                ApplicationFinishDate = dto.DateFinish,
                CustomerBin = dto.CustomerBin,
                SourceLink =  dto.DetailsLink,
                LotsAmount =  dto.FullPrice ?? 0,
                LotsQuantity = dto.LotAmount ?? 0,
                SourceId = 3,
                EmailAddress = dto.ContactEmail,
                PhoneNumber = dto.ContactPhone,
                RelevanceDate = dto.RelevanceDate,
                PublishDate = dto.DateStart
            };
            // announcement.SourceLink = $"http://reestr.nadloc.kz/ru/protocol/announce/{dto.FullId}";
            if (dto.Status != null)
            {
                if (_statuses.TryGetValue(dto.Status, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }
            if (dto.PurchaseMethod != null)
            {
                if (_methods.TryGetValue(dto.PurchaseMethod, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }
            
            if (dto.KonkursDocLink != null)
            {
                announcement.Documentations = new List<AnnouncementDocumentation>
                {
                    new AnnouncementDocumentation
                    {
                        Name = dto.KonkursDocName, SourceLink = dto.KonkursDocLink, DocumentationTypeId = 3,
                        Location = dto.KonkursDocPath, RelevanceDate = dto.RelevanceDate
                    }
                };
            }
            
            announcement.Lots = new List<AdataLot>();
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    // AnnouncementId = announcement.Id,
                    Title = dtoLot.ScpDescription,
                    StatusId = announcement.StatusId,
                    MethodId = announcement.MethodId,
                    SourceId = 3,
                    ApplicationStartDate = announcement.ApplicationStartDate,
                    ApplicationFinishDate = announcement.ApplicationFinishDate,
                    CustomerBin = announcement.CustomerBin ,
                    SupplyLocation = dtoLot.DeliveryPlace,
                    TenderLocation = null, 
                    Characteristics = dtoLot.TruDescription,
                    Quantity = dtoLot.Quantity ?? 0,
                    TotalAmount = dtoLot.FullPrice ?? 0,
                    Terms = dtoLot.RequiredContractTerm,
                    SourceNumber = dtoLot.LotRowId + "-" + dtoLot.LotRowOrder,
                    TruCode = dtoLot.ScpCode,
                    RelevanceDate = dtoLot.RelevanceDate,
                    SourceLink = announcement.SourceLink
                };
                if (lot.Quantity > 0 && lot.TotalAmount > 0)
                {
                    lot.UnitPrice = lot.TotalAmount / lot.Quantity;
                }
                if (dtoLot.Unit != null)
                {
                    if (_measures.TryGetValue(dtoLot.Unit, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }
                /*if (dtoLot.ScpCode != null)
                {
                    var tru = await webTenderContext.TruCodes.FirstOrDefaultAsync(x => x.Code == dtoLot.ScpCode);
                    if (tru != null)
                        lot.TruId = tru.Id;
                }*/
                lot.Documentations = new List<LotDocumentation>();
                if (dtoLot.TechDocLink != null)
                    lot.Documentations.Add(new LotDocumentation
                    {
                        Name = dtoLot.TechDocName, SourceLink = dtoLot.TechDocLink, DocumentationTypeId = 1,
                        Location = dtoLot.TechDocPath,
                        RelevanceDate = dtoLot.RelevanceDate
                    });
                if (dtoLot.ContractDocLink != null)
                {
                    lot.Documentations.Add(new LotDocumentation
                    {
                        Name = dtoLot.ContractDocName, SourceLink = dtoLot.ContractDocLink, DocumentationTypeId = 2,
                        Location = dtoLot.ContractDocPath,
                        RelevanceDate = dtoLot.RelevanceDate
                    });
                }
                announcement.Lots.Add(lot);
            }
            return announcement;
        }
    }
}