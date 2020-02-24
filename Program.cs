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
        private static Logger logger;
        private static Dictionary<string, MigrationService> _migrations = new Dictionary<string, MigrationService>();
        public static int NumOfErrors = 0;

        public static List<Type> migrationServices = new List<Type>
        {
            typeof(LotGoszakupMigrationService)
        };

        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            logger = LogManager.GetCurrentClassLogger();
            var migrationSystem = new MigrationSystem(args);
            await migrationSystem.StartMigrations();
        }
    }
}