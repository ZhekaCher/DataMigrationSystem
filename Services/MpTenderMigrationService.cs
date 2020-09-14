using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class MpTenderMigrationService : MigrationService
    {
        private int _total=0;
        private readonly object _lock = new object();

        public MpTenderMigrationService(int numOfThreads = 10)
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
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            await using var parsedAnnouncementMpContext = new ParsedMpTenderContext();
            await parsedAnnouncementMpContext.Database.ExecuteSqlRawAsync("truncate table avroradata.mp_advert, avroradata.mp_lots, avroradata.mp_lot_file restart identity");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var parsedMpTenderContext = new ParsedMpTenderContext();
            var mpTenderDtos = parsedMpTenderContext.MpTender
                .AsNoTracking()
                .Where(t => t.Id % NumOfThreads == threadNum)
                .Include(x => x.Lots)
                .ThenInclude(x => x.Documentations);
            foreach (var dto in mpTenderDtos)
            {
                await using var webTenderContext = new AdataTenderContext();
                webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var announcement = await DtoToWebAnnouncement(webTenderContext, dto);
                try
                {
                    var found = webTenderContext.AdataAnnouncements.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                        .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                    if (found != null)
                    {
                        await webTenderContext.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                            .RunAsync();
                        foreach (var lot in announcement.Lots)
                        {
                            lot.AnnouncementId = found.Id;
                            var foundLot = webTenderContext.AdataLots.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                                .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                            if (foundLot != null)
                            {
                                await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                    .RunAsync();
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

            Logger.Info("Completed thread");
            
        }

        private async Task<AdataAnnouncement> DtoToWebAnnouncement(AdataTenderContext webTenderContext, MpTenderDto dto)
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
                var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.StatusOfAuc);
                if (status != null) 
                    announcement.StatusId = status.Id;
            }
            if (dto.TypeOfAuction != null)
            {
                var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.TypeOfAuction);
                if (method != null) 
                    announcement.MethodId = method.Id;
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
                    CustomerBin = dto.Bin
                };
                try
                {    
                    lot.Quantity = (double) dtoLot.Amount;
                }
                catch (Exception)
                {
                    Console.WriteLine();
                }
                lot.SourceLink = $"https://mp.kz/tender/{lot.SourceNumber}-{lot.Title})";
                if (dtoLot.StatusOfAuc != null)
                {
                    var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dtoLot.StatusOfAuc);
                    if (status != null) 
                        lot.StatusId = status.Id;
                }
                if (dtoLot.TypeOfAuction != null)
                {
                    var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dtoLot.TypeOfAuction);
                    if (method != null) 
                        lot.MethodId = method.Id;
                }
                if (dtoLot.UnitOfAmount != null)
                {
                    var measure = await webTenderContext.Measures.FirstOrDefaultAsync(x => x.Name == dtoLot.UnitOfAmount);
                    if (measure != null) 
                        lot.MeasureId = measure.Id;
                }
                lot.Documentations = new List<LotDocumentation>();
                foreach (var document in dtoLot.Documentations.Select(fileDto => new LotDocumentation
                {
                    Name = fileDto.Name,
                    Location = fileDto.LocalFilePath,
                    SourceLink = fileDto.FilePath,
                    DocumentationTypeId = webTenderContext.DocumentationTypes.FirstOrDefault(x=>x.Name == fileDto.Name)?.Id
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
            await using var webTenderContext = new AdataTenderContext();
            await using var parsedMpTenderContext = new ParsedMpTenderContext();
            _total = await parsedMpTenderContext.MpTender.CountAsync();
            var units = parsedMpTenderContext.Lots.Select(x=> new Measure {Name = x.UnitOfAmount}).Distinct();
            var documentationTypes = parsedMpTenderContext.MpTenderFiles.Select(x => new DocumentationType {Name = x.Name}).Distinct();
            await webTenderContext.DocumentationTypes.UpsertRange(documentationTypes).On(x => x.Name).RunAsync();
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).RunAsync(); 
            var statuses = parsedMpTenderContext.MpTender.Select(x => new Status{Name = x.StatusOfAuc}).Distinct();
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync(); 
            
        }
    }
}