using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using NLog.Config;
using Npgsql;

namespace DataMigrationSystem
{
    internal static class Program
    {
        private static Logger _logger;
        public static int NumOfErrors = 0;

        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info("Configuring and starting migrations...");
            try
            {
                var migrationSystem = new MigrationSystem(args);
                await migrationSystem.StartMigrations();
                _logger.Info($"Migration ended with '{NumOfErrors}' errors");
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                throw;
            }
        }
    }
}