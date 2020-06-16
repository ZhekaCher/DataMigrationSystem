﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GoszakupTenderMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();

        public GoszakupTenderMigrationService(int numOfThreads = 10)
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
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            var nadlocDtos = parsedAnnouncementNadlocContext.AnnouncementNadlocDtos
                .Where(t => t.Id % NumOfThreads == threadNum)
                .Include(x=>x.Lots);
            _total = nadlocDtos.Count();
            foreach (var dto in nadlocDtos)
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
                        await webTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                        await webTenderContext.AnnouncementContacts.AddAsync(
                            new AnnouncementContact {
                                PhoneNumber = dto.ContactPhone,
                                EmailAddress = dto.ContactEmail,
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
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            var units = parsedAnnouncementNadlocContext.LotNadlocDtos
                .Select(x=>x.Unit)
                .Distinct()
                .Select(x=> new Measure {
                    Name = x
                });
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).RunAsync();
        }
        private async Task<AdataAnnouncement> DtoToWebAnnouncement(AnnouncementNadlocDto dto)
        {
            await using var webTenderContext = new AdataTenderContext();

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
                SourceId = 3
            };
            if (dto.Status != null)
            {
                var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.Status);
                if (status != null) 
                    announcement.StatusId = status.Id;
            }
            if (dto.PurchaseMethod != null)
            {
                var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.Status);
                if (method != null) 
                    announcement.MethodId = method.Id;
            }
            
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    AnnouncementId = announcement.Id,
                    Title = dtoLot.ScpDescription,
                    StatusId = announcement.StatusId,
                    MethodId = announcement.MethodId,
                    SourceId = 3,
                    ApplicationStartDate = announcement.ApplicationStartDate,
                    ApplicationFinishDate = announcement.ApplicationFinishDate,
                    CustomerBin = announcement.CustomerBin ,
                    SupplyLocation = dtoLot.DeliveryPlace,
                    TenderLocation = null, 
                    TruCode = null,
                    Characteristics = dtoLot.TruDescription,
                    Quantity = dtoLot.Quantity ?? 0,
                    TotalAmount = dtoLot.FullPrice ?? 0,
                    Terms = dtoLot.RequiredContractTerm,
                    SourceLink = dtoLot.ContractDocLink
                };
                if (lot.Quantity > 0 && lot.TotalAmount > 0)
                {
                    lot.UnitPrice = lot.TotalAmount / lot.Quantity;
                }
                if (dtoLot.Unit != null)
                {
                    var measure = await webTenderContext.Measures.FirstOrDefaultAsync(x => x.Name == dtoLot.Unit);
                    if (measure != null) 
                        lot.MeasureId = measure.Id;
                }
                announcement.Lots.Add(lot);
            }
            return announcement;
        }
    }
}