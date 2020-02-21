using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public override async Task StartMigratingAsync(int numOfThreads = 30)
        {
            
            Logger.Warn(numOfThreads);
            Logger.Info("Start");
            var tasks = new List<Task>();
            for (var i = 0; i < numOfThreads; i++)
                tasks.Add(Migrate(numOfThreads, i));

            await Task.WhenAll(tasks);
            Logger.Info("Stop");
        }

        private async Task Migrate(int numOfThreads, int threadNum)
        {
            await using var webAnnouncementContext = new WebAnnouncementContext();
            await using var parsedAnnouncementGoszakupContext = new ParsedAnnouncementGoszakupContext();

            var a = parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos;
            await foreach (var announcementGoszakupDto in a)
            {
                var b = new Announcement();
                webAnnouncementContext.Announcements.Add()
                webAnnouncementContext.SaveChanges();
            }
            
            
            
            
            
            
            
            
            
            foreach (var dto in parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos.Where(x =>
                x.Id % numOfThreads == threadNum))
            {
                var dtoIns = AnnouncementGoszakupDtoToAnnouncement(dto);
                dtoIns.IdTf = _sTradingFloorId;
                await webAnnouncementContext.Announcements.Upsert(dtoIns).On(x => new {x.IdAnno, x.IdTf}).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
        }

        private Announcement AnnouncementGoszakupDtoToAnnouncement(AnnouncementGoszakupDto announcementGoszakupDto)
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