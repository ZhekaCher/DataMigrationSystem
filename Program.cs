using System;
using System.Text;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using NLog.Config;

namespace DataMigrationSystem
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            // LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            // Logger logger = LogManager.GetCurrentClassLogger();
            // logger.Info("Starting Migration!");

            Console.WriteLine("Starting Migration!");
            EnforcementDebtMigrationService enforcementDebtMigrationService = new EnforcementDebtMigrationService();
            await enforcementDebtMigrationService.StartMigratingAsync();
            Console.WriteLine("Done!");
        }
    }
}