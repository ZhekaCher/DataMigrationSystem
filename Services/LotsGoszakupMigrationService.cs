using System;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 11:08:36
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class LotsGoszakupMigrationService : MigrationService
    {
        private readonly ParsedLotsGoszakupContext _parsedLotsGoszakupContext;
        private readonly WebLotsContext _webLotsContext;


        public LotsGoszakupMigrationService()
        {
            _parsedLotsGoszakupContext = new ParsedLotsGoszakupContext();
            _webLotsContext = new WebLotsContext();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await Migrate();
        }
        
        private async Task Migrate()
        {
            Logger.Info("Started loading");
            
            Logger.Info("Completed loading");
        }
    }
}