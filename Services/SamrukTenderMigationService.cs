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
    public class SamrukTenderMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();

        public SamrukTenderMigrationService(int numOfThreads = 10)
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
            // await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            // await parsedAnnouncementNadlocContext.Database.ExecuteSqlRawAsync(
            // "truncate table avroradata.nadloc_tenders");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var webTenderContext = new AdataTenderContext();
            await using var parsedSamrukContext = new ParsedSamrukContext();
            var samrukAdvertDtos = parsedSamrukContext.SamrukAdverts
                .Where(t => t.Id % NumOfThreads == threadNum)
                .Include(x=>x.Lots);
            _total = samrukAdvertDtos.Count();
            foreach (var dto in samrukAdvertDtos)
            {
                var announcement = await DtoToWebAnnouncement(dto);
                try
                {
                    var found = webTenderContext.AdataAnnouncements
                        .Include(x=>x.Lots)
                        .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                    if (found != null)
                    {
                        webTenderContext.AdataLots.RemoveRange(found.Lots);
                        await webTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                        await webTenderContext.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                            .RunAsync();
                    }
                    else
                    {
                        await webTenderContext.AdataAnnouncements.AddAsync(announcement);
                        announcement.Lots.ForEach(x=>x.AnnouncementId = announcement.Id);
                        await webTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                        await webTenderContext.AnnouncementContacts.AddAsync(
                            new AnnouncementContact {
                                PhoneNumber = dto.Phone,
                                EmailAddress = dto.Email,
                                AnnouncementId = announcement.Id
                            }
                        );
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

        private async Task MigrateReferences()
        {
            await using var webTenderContext = new AdataTenderContext();
            await using var parsedAnnouncementNadlocContext = new ParsedSamrukContext();
            var units = parsedAnnouncementNadlocContext.Lots
                .Select(x=>x.MkeiRussian)
                .Distinct()
                .Select(x=> new Measure {
                    Name = x
                });
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).RunAsync();
        }
        private async Task<AdataAnnouncement> DtoToWebAnnouncement(SamrukAdvertDto dto)
        {
            await using var webTenderContext = new AdataTenderContext();

            var announcement = new AdataAnnouncement
            {
                SourceNumber =  dto.AdvertId.ToString(),
                Title =  dto.NameRussian,
                ApplicationStartDate =  dto.AcceptanceBeginDatetime,
                ApplicationFinishDate = dto.AcceptanceEndDatetime,
                CustomerBin = long.Parse(dto.CustomerBin),
                SourceLink =  null,
                LotsAmount =  dto.SumTruNoNds ?? 0,
                // LotsQuantity = dto.LotAmount ?? 0,
                SourceId = 1
            };
            if (dto.AdvertStatus != null)
            {
                var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.AdvertStatus);
                if (status != null) 
                    announcement.StatusId = status.Id;
            }
            if (dto.TenderType != null)
            {
                var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.TenderType);
                if (method != null) 
                    announcement.MethodId = method.Id;
            }
            
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    // AnnouncementId = announcement.Id,
                    Title = dtoLot.TruDetailRussian,
                    StatusId = announcement.StatusId,
                    MethodId = announcement.MethodId,
                    SourceId = 1,
                    ApplicationStartDate = announcement.ApplicationStartDate,
                    ApplicationFinishDate = announcement.ApplicationFinishDate,
                    CustomerBin = announcement.CustomerBin ,
                    SupplyLocation = dtoLot.DeliveryLocation,
                    TenderLocation = dtoLot.TenderLocation, 
                    TruCode = null,
                    Characteristics = dtoLot.TruDetailRussian,
                    Quantity = double.Parse(dtoLot.Count),
                    TotalAmount = dtoLot.SumTruNoNds ?? 0,
                    Terms = null,
                    SourceLink = null
                };
                if (lot.Quantity > 0 && lot.TotalAmount > 0)
                {
                    lot.UnitPrice = lot.TotalAmount / lot.Quantity;
                }
                if (dtoLot.MkeiRussian != null)
                {
                    var measure = await webTenderContext.Measures.FirstOrDefaultAsync(x => x.Name == dtoLot.MkeiRussian);
                    if (measure != null) 
                        lot.MeasureId = measure.Id;
                }
                announcement.Lots.Add(lot);
            }
            return announcement;
        }
    }
}