using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
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

        private ParsedAnnouncementGoszakupContext _announcementGoszakupContext;
        private WebAnnouncementContext _webAnnouncementContext;

        public AnnouncementGoszakupMigrationService()
        {
            _announcementGoszakupContext = new ParsedAnnouncementGoszakupContext();
            _webAnnouncementContext = new WebAnnouncementContext();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            
        }
    }
}