using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class KaspiAuctionMigrationService : MigrationService
    {
        private int _total=0;
        private readonly object _lock = new object();
        
        public KaspiAuctionMigrationService(int numOfThreads = 5)
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
            
            await using var parsedKaspiAuctionContext = new ParsedKaspiAuctionContext();
            await using var web = new WebTenderContext();
            var tasks = new List<Task>(); 
            
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            await web.SaveChangesAsync();
            
            await CheckAuction();
            await web.SaveChangesAsync();
            
            Logger.Info("End of migration");
            await parsedKaspiAuctionContext.Database.ExecuteSqlRawAsync("truncate table avroradata.kaspi_auction restart identity");
        }
        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var parsed = new ParsedKaspiAuctionContext();
            await using var web = new WebTenderContext();
            foreach (var kaspiAuctionDto in parsed.KaspiAuctionDtos.Where(x => x.Id % NumOfThreads == threadNum))
            {
                var announcement = new AdataAnnouncement
                {
                    SourceNumber = kaspiAuctionDto.Number,
                    Title = kaspiAuctionDto.Name,
                    StatusId = kaspiAuctionDto.StatusMembers=="Прием заявок"?3:61,
                    MethodId = 9,
                    ApplicationStartDate = kaspiAuctionDto.TradingStartDate,
                    ApplicationFinishDate = kaspiAuctionDto.ApplicationDeadline,
                    CustomerBin = kaspiAuctionDto.Bin,
                    SourceId = 8,
                    LotsAmount = kaspiAuctionDto.StartCost,
                    SourceLink = kaspiAuctionDto.SourceLink,
                    RelevanceDate = DateTime.Now
                };
                await web.AdataAnnouncements.Upsert(announcement).On(x => new{x.SourceNumber, x.SourceId}).RunAsync();
                lock (_lock)
                {
                    Logger.Trace(_total--);
                }
            }

        }

        private async Task CheckAuction()
        {
            await using var web = new WebTenderContext();
            var webannouncement = web.AdataAnnouncements.Where(x => x.SourceId == 8);
            foreach (var adataAnnouncement in webannouncement)
            {
                if (adataAnnouncement.RelevanceDate<DateTime.Today)
                {
                    var announcement = new AdataAnnouncement
                    {
                    StatusId = 19
                    };
                    await web.AdataAnnouncements.Upsert(announcement).On(x => new{x.SourceNumber, x.SourceId}).RunAsync();
                }
            }
        }
    }
}