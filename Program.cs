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


// FOR 200 COMMIT
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
            Console.WriteLine("\n"+
             "//                       _oo0oo_\n"+
             "//                      o8888888o\n"+
             "//                      88\" . \"88\n"+
             "//                      (| -_- |)\n"+
             "//                      0\\  =  /0\n"+
             "//                    ___/`---'\\___\n"+
             "//                  .' \\\\|     |// '.\n"+
             "//                 / \\\\|||  :  |||// \\n"+
             "  //             / _||||| -:- |||||- \\n"+
             "  //            |   | \\\\\\  -  /// |   |\n"+
             "  //            | \\_|  ''\\---/''  |_/ |\n"+
             "  //            \\  .-\\__  '-'  ___/-. /\n"+
             "  //          ___'. .'  /--.--\\  `. .'___\n"+
             "  //       .\"\" '<  `.___\\_<|>_/___.' >' \"\".\n"+
             "  //      | | :  `- \\`.;`\\ _ /`;.`/ - ` : | |\n"+
             "  //      \\  \\ `_.   \\_ __\\ /__ _/   .-` /  /\n"+
             "  //  =====`-.____`.___ \\_____/___.-`___.-'=====\n"+
             "  //                       `=---='\n"+
             "  // \n"+
             "  // \n"+
             "  //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n"+
             " // \n"+
             " //            God Bless         No Bugs\n" +
             " //\n");
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