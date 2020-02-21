using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using NLog.Config;

namespace DataMigrationSystem
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Starting Migration!");
            var migrationService = new LotGoszakupMigrationService(1);
            var lists = new List<MigrationService>();
            lists.Add(migrationService);
            await 
                 lists[0].StartMigratingAsync();
            logger.Info("Done!");
        }
    }
}