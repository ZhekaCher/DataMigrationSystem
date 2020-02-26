using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Fluent;
using SomeNamespace;

namespace DataMigrationSystem.Services
{
    public class AnnouncemenetNadlocMigrationService : MigrationService
    {
        private readonly string _currentTradingFloor="nedro";
        private int _sTradingFloorId;
        private int _total;
        private object _lock = new object();

        public AnnouncemenetNadlocMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedAnnouncementNadlocContext = new ParsedAnnouncementNadlocContext();
            using var webAnnouncememntContext = new WebAnnouncementContext();
            _total = parsedAnnouncementNadlocContext.AnnouncementNadlocDtos.Count();
            _sTradingFloorId = webAnnouncememntContext.STradingFloors
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
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var webAnnouncementContext = new WebAnnouncementContext();
            await using var parsedAnnouncementNadlocContext = new ParsedAnnouncementNadlocContext();
            foreach (var dto in parsedAnnouncementNadlocContext.AnnouncementNadlocDtos.Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                var dtoIns = DtoToWeb(dto);
                dtoIns.IdTf = _sTradingFloorId;
                await webAnnouncementContext.Announcements.Upsert(dtoIns).On(x => new {x.IdAnno, x.IdTf}).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }

            Logger.Info("Completed thread");

        }

        private Announcement DtoToWeb(AnnouncementNadlocDto announcementNadlocDto)
        {
            var announcement = new Announcement();
            announcement.IdAnno = announcementNadlocDto.TenderId;
            announcement.IdTf = _sTradingFloorId;
            announcement.NumberAnno = announcementNadlocDto.FullId;
            announcement.NameRu = announcementNadlocDto.Name;
            announcement.DtStart = announcementNadlocDto.DateStart;
            announcement.DtEnd = announcementNadlocDto.DateFinish;
            announcement.CustomerBin = announcementNadlocDto.CustomerBin;
            announcement.OrganizerBin = announcementNadlocDto.CustomerBin;
            announcement.SumTruNoNds = announcementNadlocDto.FullPrice;
            return announcement;
        }
    }
}