using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using NLog.Config;

namespace DataMigrationSystem
{
    internal static class Program
    {
        private static Logger logger;
        private static Dictionary<string, MigrationService> _migrations = new Dictionary<string, MigrationService>();

        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            logger = LogManager.GetCurrentClassLogger();

            _migrations.Add("announcement_goszakup", new AnnouncementGoszakupMigrationService());
            _migrations.Add("court_case", new CourtCaseMigrationService());
            _migrations.Add("enforcement_debt", new EnforcementDebtMigrationService());
            _migrations.Add("leaving_restriction", new LeavingRestrictionMigrationService());
            _migrations.Add("lot_goszakup", new LotGoszakupMigrationService());
            _migrations.Add("tax_debt", new TaxDebtMigrationService());
            _migrations.Add("wanted_individual", new WantedIndividualMigrationService());

            await ProceedArguments(args);

            // logger.Info("Starting Migration!");
            // var migrationService = new LotGoszakupMigrationService(1);
            // var lists = new List<MigrationService>();
            // lists.Add(migrationService);
            // logger.Info("Done!");
        }

        private static async Task ProceedArguments(string[] args)
        {
            var log = "Data migration started; Next migrations will be proceed:";
            var numOfErrors = 0;
            if (args.Length == 0)
            {
                log = _migrations.Aggregate(log, (current, keyValuePair) => current + keyValuePair.Key + "; ");
                
                logger.Info(log);
                
                foreach (var keyValuePair in _migrations)
                {
                    try
                    {
                        await keyValuePair.Value.StartMigratingAsync();
                    }
                    catch (Exception e)
                    {
                        ++numOfErrors;
                        logger.Error($"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                    }
                }
            }
            else
            {
                if (args[1].ToLower().Equals("--help"))
                {
                    //TODO()
                }
            }
            
            
            logger.Info($"Data migration fully completed with '{numOfErrors}' errors");
        }
    }
}