using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class EtsTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();

        public EtsTenderMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");

            await using var parsedEtsTenderContext = new ParsedEtsTenderContext();
            _total = await parsedEtsTenderContext.AnnouncementEtsTenderDtos.CountAsync();
            await Migrate();
           
           
            await using var webTenderContext = new  WebTenderContext();
            var starDate = await parsedEtsTenderContext.AnnouncementEtsTenderDtos.MinAsync(x => x.RelevanceDate);
            
            await webTenderContext.AdataAnnouncements.Where(x => x.SourceId == 5 && x.RelevanceDate < starDate ).ForEachAsync(x => x.StatusId = 38);
            await webTenderContext.AdataLots.Where(x => x.SourceId == 5 && x.RelevanceDate < starDate ).ForEachAsync(x => x.StatusId = 38);
            await webTenderContext.SaveChangesAsync();
            
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");
            
            await parsedEtsTenderContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.ets_announcements ,avroradata.ets_lots ,avroradata.ets_purchasing_positions");
            Logger.Info("Truncated");
            Logger.Info("End of migration");
            
        }

        private async Task Migrate()
        {
            Logger.Info("started thread");

            await using var parsedEtsTenderContext = new ParsedEtsTenderContext();
            var etsTenderDtos = parsedEtsTenderContext.AnnouncementEtsTenderDtos.AsNoTracking().Include(x => x.Lots);
    
            var tasks = new List<Task>();
            foreach (var dto in etsTenderDtos)
            {
                await Task.Delay(20);
                tasks.Add(Insert(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task Insert(AnnouncementEtsTenderDto dto)
        {
            await using var adataTenderContext = new WebTenderContext();
            adataTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var announcement = await DtoToWebAnnouncement(adataTenderContext, dto);
            try
            {
                var found = adataTenderContext.AdataAnnouncements.Include(x=>x.Lots)
                    .FirstOrDefault(x =>
                        x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
                {
                    if (announcement.StatusId != 1 && found.StatusId == 1)
                    {
                        found.StatusId = announcement.StatusId;
                        found.RelevanceDate = announcement.RelevanceDate;
                        await adataTenderContext.AdataLots.Where(x => x.AnnouncementId == found.Id).ForEachAsync(x =>
                        {
                            x.StatusId = announcement.StatusId;
                            x.RelevanceDate = announcement.RelevanceDate;
                        });
                        await adataTenderContext.SaveChangesAsync();
                    }
                    else
                    {
                        adataTenderContext.AdataLots.RemoveRange(found.Lots);
                        await adataTenderContext.SaveChangesAsync();
                        announcement.Lots.ForEach(x => x.AnnouncementId = found.Id);
                        await adataTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                        await adataTenderContext.SaveChangesAsync();
                        await adataTenderContext.AdataAnnouncements.Upsert(announcement)
                            .On(x => new {x.SourceNumber, x.SourceId})
                            .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.LotsQuantity != y.LotsQuantity || x.MethodId != y.MethodId || x.TenderPriorityId != y.TenderPriorityId || x.RelevanceDate != y.RelevanceDate).RunAsync();
                    }
                }
                else
                {
                    await adataTenderContext.AdataAnnouncements.AddAsync(announcement);
                    await adataTenderContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }


        private static async Task<AdataAnnouncement> DtoToWebAnnouncement(WebTenderContext webTenderContext,
            AnnouncementEtsTenderDto dto)
        {
            // await using var webTenderContext = new AdataTenderContext();
            var announcement = new AdataAnnouncement
            {
                SourceNumber = dto.SourceCode,
                Title = dto.Title,
                ApplicationStartDate = dto.StartDate,
                ApplicationFinishDate = dto.FinishDate,
                CustomerBin = dto.CustomerBin,
                SourceLink = dto.SourceLink,
                LotsAmount = dto.FullPrice,
                LotsQuantity = dto.Lots.Count,
                SourceId = 5,
                RelevanceDate = dto.RelevanceDate,
                PublishDate = dto.StartDate,
                PhoneNumber = dto.ContactPerson
            };

            if (dto.Status)
            {
                announcement.StatusId = 1;
            }else
                announcement.StatusId = 38;
            if (dto.PurchaseType != null)
            {
                var method = await webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.PurchaseType);
                if (method != null)
                    announcement.MethodId = method.Id;
            }

            announcement.Lots = new List<AdataLot>();
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    Title = dtoLot.LotName,
                    StatusId = announcement.StatusId,
                    MethodId = announcement.MethodId,
                    SourceId = 5,
                    ApplicationStartDate = announcement.ApplicationStartDate,
                    ApplicationFinishDate = announcement.ApplicationFinishDate,
                    CustomerBin = announcement.CustomerBin,
                    SupplyLocation = dto.DeliveryPlace,
                    TenderLocation = dto.ProcedurePlace,
                    Characteristics = dto.Rubrics,
                    Quantity = 0,
                    TotalAmount = dtoLot.FullPrice,
                    Terms = dto.PaymentConditions,
                    SourceNumber = announcement.SourceNumber + "-"+ dtoLot.LotNumber,
                    RelevanceDate = dto.RelevanceDate,
                    SourceLink = dto.SourceLink
                    
                };
                if (lot.Quantity > 0 && lot.TotalAmount > 0)
                {
                    lot.UnitPrice = lot.TotalAmount / lot.Quantity;
                }

                if (dtoLot.LotName != null)
                {
                    var tru = await webTenderContext.TruCodes.FirstOrDefaultAsync(x => x.Code == dtoLot.LotName);
                    if (tru != null)
                        lot.TruCode = tru.Code;
                }

                lot.Documentations = new List<LotDocumentation>();
                announcement.Lots.Add(lot);
            }

            return announcement;
        }
    }
}