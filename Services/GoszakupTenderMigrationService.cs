﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GoszakupTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly int _sourceId = 2;
        private Status[] webStatuses;

        private Method[] webMethods;

        // private List<RefLotStatusGoszakupDto> refLotStatuses;
        // private List<RefBuyStatusGoszakupDto> refBuyStatuses;
        // private List<RefTradeMethodGoszakupDto> refTradeMethods;
        //TODO(PRELOAD REFERENCES)
        public GoszakupTenderMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedGoszakupContext();
            _total = parsingContext.AnnouncementGoszakupDtos.Count();

            NumOfThreads = numOfThreads;

            using var webTenderContext = new AdataTenderContext();


            webStatuses = webTenderContext.Statuses.ToArray();
            webMethods = webTenderContext.Methods.ToArray();
            // refLotStatuses = parsingContext.RefLotStatusGoszakupDtos.ToList();
            // refBuyStatuses = parsingContext.RefBuyStatusGoszakupDtos.ToList();
            // refTradeMethods = parsingContext.RefTradeMethodGoszakupDtos.ToList();
            //FirstOrDefault(x => x.Name == announcementGoszakupDto.RefTradeMethod.NameRu);
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");
            await using var parsedAnnouncementGoszakupContext = new ParsedGoszakupContext();
            parsedAnnouncementGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;
            foreach (var dto in parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos.AsNoTracking().Where(x =>
                    x.Id % NumOfThreads == threadNum)
                .Include(x => x.Lots)
                .ThenInclude(x => x.RefLotStatus)
                .Include(x => x.Lots)
                .ThenInclude(x => x.RefTradeMethod)
                .Include(x => x.RefTradeMethod)
                .Include(x => x.RefBuyStatus)
            )
            {
                try
                {
                    
                    await using var webTenderContext = new AdataTenderContext();
                    webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    var announcement = DtoToWebAnnouncement(dto);
                    
                    var found = webTenderContext.AdataAnnouncements
                        .FirstOrDefault(x =>
                            x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                    if (found != null)
                    {
                        await webTenderContext.AdataAnnouncements.Upsert(announcement)
                            .On(x => new {x.SourceNumber, x.SourceId})
                            .RunAsync();
                        foreach (var lot in announcement.Lots)
                        {
                            lot.AnnouncementId = found.Id;
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
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
                    if (e.Message.Contains("violates foreign key") || e.Message.Contains("updating the entries"))
                    {
                        // Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
                    }
                    else
                    {
                        Logger.Error(e);
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total} {Thread.CurrentThread.Name}");
            }

            Logger.Info($"Completed thread at {_total}");
        }

        private AdataAnnouncement DtoToWebAnnouncement(AnnouncementGoszakupDto announcementGoszakupDto)
        {
            using var webTenderContext = new AdataTenderContext();


            var status =
                webStatuses?.FirstOrDefault(
                    x => x.Name == announcementGoszakupDto.RefBuyStatus?.NameRu);
            var method =
                webMethods?.FirstOrDefault(
                    x => x.Name == announcementGoszakupDto.RefTradeMethod?.NameRu);

            var announcement = new AdataAnnouncement
            {
                SourceNumber = announcementGoszakupDto.NumberAnno,
                Title = announcementGoszakupDto.NameRu,

                //Refactor
                StatusId = status?.Id,
                MethodId = method?.Id,

                //TODO(ApplicationStartDate)
                //TODO(ApplicationFinishDate)
                CustomerBin = announcementGoszakupDto.OrgBin,
                LotsQuantity = announcementGoszakupDto.Lots.Count,
                LotsAmount = (double) announcementGoszakupDto.TotalSum,
                SourceLink = $"https://www.goszakup.gov.kz/ru/announce/index/{announcementGoszakupDto.Id}",
                RelevanceDate = announcementGoszakupDto.Relevance,
                SourceId = _sourceId
            };
            announcement.Lots = new List<AdataLot>();

            foreach (var lotGoszakupDto in announcementGoszakupDto.Lots)
                announcement.Lots.Add(DtoToWebLot(lotGoszakupDto, webTenderContext));

            return announcement;
        }

        private AdataLot DtoToWebLot(LotGoszakupDto lotGoszakupDto, AdataTenderContext webTenderContext)
        {
            var status =
                webStatuses?.FirstOrDefault(
                    x => x.Name == lotGoszakupDto.RefLotStatus?.NameRu);
            var method =
                webMethods?.FirstOrDefault(
                    x => x.Name == lotGoszakupDto.RefTradeMethod?.NameRu);
            var lot = new AdataLot
            {
                SourceNumber = lotGoszakupDto.TrdBuyNumberAnno + "-" + lotGoszakupDto.LotNumber,
                StatusId = status?.Id,
                MethodId = method?.Id,
                SourceId = _sourceId,
                // Application start date
                // Application finish date
                CustomerBin = lotGoszakupDto.CustomerBin,
                // Supply location
                // Tender location
                // TruId
                Characteristics = lotGoszakupDto.DescriptionRu,
                UnitPrice = (double) (lotGoszakupDto.Amount / lotGoszakupDto.Count),
                TotalAmount = (double) lotGoszakupDto.Amount,
                // Terms
                RelevanceDate = lotGoszakupDto.Relevance
            };

            if (lotGoszakupDto.Count != null) lot.Quantity = (double) lotGoszakupDto.Count;
            if (lotGoszakupDto.Amount != null) lot.TotalAmount = (double) lotGoszakupDto.Amount;
            return lot;
        }
    }
}