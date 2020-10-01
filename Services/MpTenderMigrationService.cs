using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class MpTenderMigrationService : MigrationService
    {
        private int _total=0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        
        public MpTenderMigrationService(int numOfThreads = 5)
        {
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
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
            
            await using var parsedAnnouncementMpContext = new ParsedMpTenderContext();
            await parsedAnnouncementMpContext.Database.ExecuteSqlRawAsync("truncate table avroradata.mp_advert, avroradata.mp_lots, avroradata.mp_lot_file restart identity");
        }

        private async Task Insert(MpTenderDto dto)
        {
            await using var webTenderContext = new WebTenderContext();
            webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var announcement = DtoToWebAnnouncement(dto);
            try
            {
                var found = webTenderContext.AdataAnnouncements.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                    .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
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
                                .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.Characteristics != y.Characteristics || x.MethodId != y.MethodId || x.MeasureId != y.MeasureId || x.SupplyLocation != y.SupplyLocation).RunAsync();
                            /*lot.PaymentCondition.LotId = foundLot.Id;
                            await webTenderContext.PaymentConditions.Upsert(lot.PaymentCondition).On(x => x.LotId)
                                .RunAsync();*/
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
                Logger.Error(e.StackTrace);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }
        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsedMpTenderContext = new ParsedMpTenderContext();
            var mpTenderDtos = parsedMpTenderContext.MpTender
                .AsNoTracking()
                .Include(x => x.Lots)
                .ThenInclude(x => x.Documentations);
            var tasks = new List<Task>();
            foreach (var dto in mpTenderDtos)
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

        private AdataAnnouncement DtoToWebAnnouncement(MpTenderDto dto)
        {
            var announcement = new AdataAnnouncement
            {
                SourceNumber =  dto.AdvertId.ToString(),
                Title =  dto.AdvertName,
                ApplicationStartDate =  dto.StartAuc,
                ApplicationFinishDate = dto.EndAuc,
                CustomerBin = dto.Bin,
                LotsAmount = dto.SumOfLots ?? 0,
                LotsQuantity = dto.Count,
                SourceId = 4,
                PublishDate = dto.StartAuc
            };
            announcement.SourceLink = $"https://mp.kz/tenders/{announcement.SourceNumber}-{announcement.Title})";
            if (dto.StatusOfAuc != null)
            {
                if (_statuses.TryGetValue(dto.StatusOfAuc, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }
            if (dto.TypeOfAuction != null)
            {
                if (_methods.TryGetValue(dto.TypeOfAuction, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }
            announcement.Lots = new List<AdataLot>();
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    SourceNumber = dtoLot.LotId.ToString(),
                    Title = dtoLot.LotName,
                    SourceId = 4,
                    ApplicationStartDate = dtoLot.StartAuc,
                    ApplicationFinishDate = dtoLot.EndAuc,
                    SupplyLocation = dtoLot.DeliveryAdres,
                    TenderLocation = dtoLot.LotRegion,
                    Characteristics = dtoLot.LotDescription,
                    TotalAmount = dtoLot.LotVolume ?? 0,
                    UnitPrice = dtoLot.OpeningPrice ?? 0,
                    Terms = dtoLot.DeliveryTime,
                    CustomerBin = dto.Bin,
                    Quantity = dtoLot.Amount ?? 0
                };
                
                lot.SourceLink = $"https://mp.kz/tender/{lot.SourceNumber}-{lot.Title})";
                if (dtoLot.StatusOfAuc != null)
                {
                    if (_statuses.TryGetValue(dtoLot.StatusOfAuc, out var temp))
                    {
                        lot.StatusId = temp;
                    }
                }
                if (dtoLot.TypeOfAuction != null)
                {
                    if (_methods.TryGetValue(dtoLot.TypeOfAuction, out var temp))
                    {
                        lot.MethodId = temp;
                    }
                }
                if (dtoLot.UnitOfAmount != null)
                {
                    if (_measures.TryGetValue(dtoLot.UnitOfAmount, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }
                lot.Documentations = new List<LotDocumentation>();
                foreach (var document in dtoLot.Documentations.Select(fileDto => new LotDocumentation
                {
                    Name = fileDto.Name,
                    Location = fileDto.LocalFilePath,
                    SourceLink = fileDto.FilePath,
                }))
                {
                    lot.Documentations.Add(document);
                }
                announcement.Lots.Add(lot);
            }
            return announcement;
        }


        private async Task MigrateReferences()
        {
            await using var webTenderContext = new WebTenderContext();
            await using var parsedMpTenderContext = new ParsedMpTenderContext();
            _total = await parsedMpTenderContext.MpTender.CountAsync();
            var units = parsedMpTenderContext.Lots.Select(x=> new Measure {Name = x.UnitOfAmount}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync(); 
            var statuses = parsedMpTenderContext.MpTender.Select(x => new Status{Name = x.StatusOfAuc}).Distinct().Where(x=>x.Name!=null);
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();
            foreach (var dict in webTenderContext.Measures)
                _measures.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Statuses)
                _statuses.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Methods)
                _methods.Add(dict.Name, dict.Id);
        }
    }
}