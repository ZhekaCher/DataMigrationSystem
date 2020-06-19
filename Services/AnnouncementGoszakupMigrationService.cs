using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:24:44
    /// @version 1.0
    /// <summary>
    /// migration of announcements
    /// </summary>
    public class AnnouncementGoszakupMigrationService : MigrationService
    {
        private readonly string _currentTradingFloor = "goszakup";
        private int _sTradingFloorId;
        private int _total;
        private object _lock = new object();


        public AnnouncementGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedAnnouncementGoszakupContext = new ParsedAnnouncementGoszakupContext();
            using var webAnnouncementContext = new WebAnnouncementContext();
            _total = parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos.Count();
            _sTradingFloorId = webAnnouncementContext.STradingFloors
                .FirstOrDefault(x => x.Code.Equals(_currentTradingFloor)).Id;
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
            await using var parsedAnnouncementGoszakupContext = new ParsedAnnouncementGoszakupContext();
            await parsedAnnouncementGoszakupContext.Database.ExecuteSqlRawAsync(
            "truncate table avroradata.announcement_goszakup restart identity cascade;");
            Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            // Logger.Info("Started thread");

            await using var webAnnouncementContext = new WebAnnouncementContext();
            await using var parsedAnnouncementGoszakupContext = new ParsedAnnouncementGoszakupContext();
            foreach (var dto in parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos.Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                var dtoIns = DtoToWeb(dto);
                dtoIns.IdTf = _sTradingFloorId;
                try
                {
                    await webAnnouncementContext.Announcements.Upsert(dtoIns).On(x => new {x.IdAnno, x.IdTf})
                        .RunAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        // Logger.Warn($"Message:|{e.Message}|; IdAnno:|{dtoIns.IdAnno}|;");
                    }
                    else
                    {
                        Logger.Error(
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; IdAnno:|{dtoIns.IdAnno}|; Id:|{dtoIns.Id}|");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
            // Logger.Info($"Completed thread at {_total}");
        }

        private Announcement DtoToWeb(AnnouncementGoszakupDto announcementGoszakupDto)
        {
            var announcement = new Announcement();
            announcement.IdAnno = announcementGoszakupDto.Id;
            announcement.NumberAnno = announcementGoszakupDto.NumberAnno;
            announcement.NameRu = announcementGoszakupDto.NameRu;
            announcement.NameKz = announcementGoszakupDto.NameKz;
            announcement.DtStart = announcementGoszakupDto.StartDate;
            announcement.DtEnd = announcementGoszakupDto.EndDate;
            announcement.CustomerBin = announcementGoszakupDto.CustomerBin;
            announcement.RelevanceDate = announcementGoszakupDto.Relevance;
            announcement.ParentId = announcementGoszakupDto.ParentId;

            return announcement;
        }
    }
}