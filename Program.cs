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
        public static int NumOfErrors = 0;

        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            logger = LogManager.GetCurrentClassLogger();


            //Creating of the services with the given numbers of threads 
            var numOfThreads = -1;

            foreach (var arg in args)
            {
                if (!arg.ToLower().StartsWith("-t") && !arg.ToLower().StartsWith("--threads")) continue;
                var temp = arg.ToLower().Replace("--threads", "");
                temp = temp.Replace("-t", "");
                int.TryParse(temp, out numOfThreads);
                if (numOfThreads >= 1 && numOfThreads <= 50) break;
                logger.Warn(
                    $"Unacceptable value for thread numbers '{arg}'; Value should correlate between 1 and 50 and match to the following form: '-t5'");
                Environment.Exit(1);
            }

            // _migrations.Add("announcement_goszakup",
            //     numOfThreads == 1
            //         ? new AnnouncementGoszakupMigrationService()
            //         : new AnnouncementGoszakupMigrationService(numOfThreads));
            // _migrations.Add("court_case",
            //     numOfThreads == 1
            //         ? new CourtCaseMigrationService()
            //         : new CourtCaseMigrationService(numOfThreads));
            // _migrations.Add("enforcement_debt",
            //     numOfThreads == 1
            //         ? new EnforcementDebtMigrationService()
            //         : new EnforcementDebtMigrationService(numOfThreads));
            // _migrations.Add("leaving_restriction",
            //     numOfThreads == 1
            //         ? new LeavingRestrictionMigrationService()
            //         : new LeavingRestrictionMigrationService(numOfThreads));
            _migrations.Add("lot_goszakup",
                numOfThreads == -1
                    ? new LotGoszakupMigrationService()
                    : new LotGoszakupMigrationService(numOfThreads));
            // _migrations.Add("tax_debt",
            //     numOfThreads == 1
            //         ? new TaxDebtMigrationService()
            //         : new TaxDebtMigrationService(numOfThreads));
            // _migrations.Add("wanted_individual",
            //     numOfThreads == 1
            //         ? new WantedIndividualMigrationService()
            //         : new WantedIndividualMigrationService(numOfThreads));
            // _migrations.Add("all_participants_goszakup",
            //     numOfThreads == -1
            //         ? new AllParticipantsGoszakupMigrationService()
            //         : new AllParticipantsGoszakupMigrationService(numOfThreads));
            // _migrations.Add("unscrupulous_goszakup",
            //     numOfThreads == -1
            //         ? new UnscrupulousGoszakupMigrationService()
            //         : new UnscrupulousGoszakupMigrationService(numOfThreads));


            await ProceedArguments(args);
        }

        private static async Task ProceedArguments(string[] args)
        {
            var commandsDictionary = new Dictionary<string, string>();

            //Generating help list
            commandsDictionary.Add("--help (-h)", "prints commands list");
            commandsDictionary.Add("--list (-l)", "prints the list of avaliable migrations");
            commandsDictionary.Add("--threads (-t)",
                $"choose number of threads\n{"Example: -t5 -> starting with 5 threads for all migrations",81}");
            commandsDictionary.Add("--migrations (-m)",
                $"allows to choose migrations\n{"Example: -m tax_debt wanted_individual -> starting the given migrations",94}");
            var helpString = commandsDictionary.Aggregate("",
                (current, keyValuePair) => current + $"{keyValuePair.Key,-20} : {keyValuePair.Value}\n");

            var startLog = "Data migration started; Next migrations will be proceed: ";
            var endLog = "Data migration fully completed with '{0}' errors";

            if (args.Length == 0)
            {
                startLog = _migrations.Aggregate(startLog,
                    (current, keyValuePair) => current + keyValuePair.Key + "; ");
                logger.Info(startLog);
                await Migrate();
                logger.Info(endLog, NumOfErrors);
            }
            else
            {
                var listFlag = false;
                List<string> argMigrations = null;
                foreach (var arg in args)
                {
                    if (arg.StartsWith("-t") || arg.StartsWith("--threads"))
                        continue;
                    if (listFlag)
                    {
                        argMigrations.Add(arg.ToLower());
                        continue;
                    }

                    switch (arg.ToLower())
                    {
                        case "--help":
                        case "-h":
                            Console.Write(helpString);
                            Environment.Exit(0);
                            break;
                        case "--list":
                        case "-l":
                            Console.WriteLine("The list of avaliable migrations:");
                            foreach (var keyValuePair in _migrations)
                                Console.WriteLine($"\t{keyValuePair.Key}");
                            Environment.Exit(0);
                            break;
                        case "--migrate":
                        case "-m":
                            listFlag = true;
                            argMigrations = new List<string>();
                            break;
                        default:
                            Console.WriteLine($"Unknown argument '{arg}'; Try this:");
                            Console.Write(helpString);
                            Environment.Exit(0);
                            break;
                    }
                }

                if (!argMigrations.Any())
                {
                    logger.Warn("No migrations has been specified");
                    Environment.Exit(0);
                }

                //Check if all given migrations exists
                foreach (var argMigration in argMigrations)
                {
                    if (_migrations.ContainsKey(argMigration)) continue;
                    logger.Warn($"Migration '{argMigration}' doesn't exists");
                    Environment.Exit(1);
                }

                startLog = argMigrations.Aggregate(startLog,
                    (current, migrationName) => current + migrationName + "; ");
                logger.Info(startLog);
                await Migrate(argMigrations);
                logger.Info(endLog, NumOfErrors);
            }
        }

        private static async Task Migrate(List<string> migrationList = null)
        {
            if (migrationList == null)
                foreach (var keyValuePair in _migrations)
                {
                    try
                    {
                        await keyValuePair.Value.StartMigratingAsync();
                    }
                    catch (Exception e)
                    {
                        ++NumOfErrors;
                        logger.Error($"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                    }
                }
            else
                foreach (var migration in migrationList)
                    try
                    {
                        await _migrations[migration].StartMigratingAsync();
                    }
                    catch (Exception e)
                    {
                        ++NumOfErrors;
                        logger.Error($"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                    }
        }
    }
}