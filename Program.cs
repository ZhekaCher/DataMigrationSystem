using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            Console.WriteLine("\n" +
                              "                 ______  ___ _____ ___  \n" +
                              "                 |  _  \\/ _ \\_   _/ _ \\ \n" +
                              "                 | | | / /_\\ \\| |/ /_\\ \\\n" +
                              "                 | | | |  _  || ||  _  |\n" +
                              "                 | |/ /| | | || || | | |\n" +
                              "                 |___/ \\_| |_/\\_/\\_| |_/\n" +
                              "        \n" +
                              "        \n" +
                              " ___  ________ _____ ______  ___ _____ _____ _____ _   _ \n" +
                              " |  \\/  |_   _|  __ \\| ___ \\/ _ \\_   _|_   _|  _  | \\ | |\n" +
                              " | .  . | | | | |  \\/| |_/ / /_\\ \\| |   | | | | | |  \\| |\n" +
                              " | |\\/| | | | | | __ |    /|  _  || |   | | | | | | . ` |\n" +
                              " | |  | |_| |_| |_\\ \\| |\\ \\| | | || |  _| |_\\ \\_/ / |\\  |\n" +
                              " \\_|  |_/\\___/ \\____/\\_| \\_\\_| |_/\\_/  \\___/ \\___/\\_| \\_/    \n\n\n" +
                              " __     _______ ____  ____ ___ ___  _   _   _   _____\n" +
                              " \\ \\   / / ____|  _ \\/ ___|_ _/ _ \\| \\ | | / | |___ / \n" +
                              "  \\ \\ / /|  _| | |_) \\___ \\| | | | |  \\| | | |   |_ \\ \n" +
                              "   \\ V / | |___|  _ < ___) | | |_| | |\\  | | |_ ___) |\n" +
                              "    \\_/  |_____|_| \\_\\____/___\\___/|_| \\_| |_(_)____/ \n\n\n" +
                              "                      _oo0oo_\n" +
                              "                     o8888888o\n" +
                              "                     88\" . \"88\n" +
                              "                     (| -_- |)\n" +
                              "                     0\\  =  /0\n" +
                              "                   ___/`---'\\___\n" +
                              "                 .' \\\\|     |// '.\n" +
                              "                / \\\\|||  :  |||// \\ \n" +
                              "               / _||||| -:- |||||- \\ \n" +
                              "              |   | \\\\\\  -  /// |   |\n" +
                              "              | \\_|  ''\\---/''  |_/ |\n" +
                              "              \\  .-\\__  '-'  ___/-. /\n" +
                              "            ___'. .'  /--.--\\  `. .'___\n" +
                              "         .\"\" '<  `.___\\_<|>_/___.' >' \"\".\n" +
                              "        | | :  `- \\`.;`\\ _ /`;.`/ - ` : | |\n" +
                              "        \\  \\ `_.   \\_ __\\ /__ _/   .-` /  /\n" +
                              "    =====`-.____`.___ \\_____/___.-`___.-'=====\n" +
                              "                         `=---='\n" +
                              "   \n" +
                              "   \n" +
                              "    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
                              "  \n" +
                              "             God Bless         No Bugs\n" +
                              " \n");
            Console.Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            _logger = LogManager.GetCurrentClassLogger();
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