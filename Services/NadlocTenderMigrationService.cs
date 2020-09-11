﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class NadlocTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();

        public NadlocTenderMigrationService(int numOfThreads = 5)
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
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            await parsedAnnouncementNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_tenders, avroradata.nadloc_lots");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            var nadlocDtos = parsedAnnouncementNadlocContext.AnnouncementNadlocDtos
                .AsNoTracking()
                .Where(t => t.Id % NumOfThreads == threadNum)
                .Include(x=>x.Lots);
            foreach (var dto in nadlocDtos)
            {
                await using var webTenderContext = new AdataTenderContext();
                webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var announcement = await DtoToWebAnnouncement(webTenderContext, dto);
                try
                {
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
                            webTenderContext.AdataLots.RemoveRange(
                                webTenderContext.AdataLots.Where(x => x.AnnouncementId == found.Id));
                            await webTenderContext.SaveChangesAsync();
                            announcement.Lots.ForEach(x => x.AnnouncementId = found.Id);
                            await webTenderContext.AdataLots.AddRangeAsync(announcement.Lots);
                            await webTenderContext.SaveChangesAsync();
                            await webTenderContext.AdataAnnouncements.Upsert(announcement)
                                .On(x => new {x.SourceNumber, x.SourceId})
                                .RunAsync();
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
            Logger.Info("Completed thread");
            
        }

        private async Task MigrateReferences()
        {
            await using var webTenderContext = new AdataTenderContext();
            await using var parsedAnnouncementNadlocContext = new ParsedNadlocContext();
            _total = await parsedAnnouncementNadlocContext.AnnouncementNadlocDtos.CountAsync();
            var units = parsedAnnouncementNadlocContext.LotNadlocDtos.Select(x=> x.Unit).Distinct().Select(x=>new Measure {Name = x});
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).RunAsync();
            var truCodes = parsedAnnouncementNadlocContext.LotNadlocDtos.Select(x=> new TruCode {Code = x.ScpCode, Name = x.ScpDescription}).Distinct();
            foreach (var truCode in truCodes)
            {
                await webTenderContext.TruCodes.UpsertRange(truCode).On(x => x.Code).RunAsync();
            }
        }
        private static async Task<AdataAnnouncement> DtoToWebAnnouncement(AdataTenderContext webTenderContext, AnnouncementNadlocDto dto)
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
                var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.Status);
                if (status != null) 
                    announcement.StatusId = status.Id;
            }
            if (dto.PurchaseMethod != null)
            {
                var method = await webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.PurchaseMethod);
                if (method != null)
                    announcement.MethodId = method.Id;
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
            int lotIndex = 0;
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
                    SourceNumber = announcement.SourceNumber + "-" + (++lotIndex),
                    TruCode = dtoLot.ScpCode,
                    RelevanceDate = dtoLot.RelevanceDate
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