using System.Threading.Tasks;
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
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
        }
    }
}