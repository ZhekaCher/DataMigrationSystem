using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using Npgsql;


// FOR 200 COMMIT
namespace DataMigrationSystem
{
    internal static class Program
    {
        private static string _title;
        public static string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    Console.Title = value;
            }
        }
        
        private static Logger _logger;
        public static int NumOfErrors = 0;

        private static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(@"
                ______   _______ _______ _______
               |   _  \ |   _   |       |   _   |
               |.  |   \|.  1   |.|   | |.  1   |
               |.  |    |.  _   `-|.  |-|.  _   |
               |:  1    |:  |   | |:  | |:  |   |
               |::.. . /|::.|:. | |::.| |::.|:. |
               `------' `--- ---' `---' `--- ---'
  ___ ___ ___ _______ _______ _______ _______ ___ _______ ______  
 |   Y   |   |   _   |   _   |   _   |       |   |   _   |   _  \ 
 |.      |.  |.  |___|.  l   |.  1   |.|   | |.  |.  |   |.  |   |
 |. \_/  |.  |.  |   |.  _   |.  _   `-|.  |-|.  |.  |   |.  |   |
 |:  |   |:  |:  1   |:  |   |:  |   | |:  | |:  |:  1   |:  |   |
 |::.|:. |::.|::.. . |::.|:. |::.|:. | |::.| |::.|::.. . |::.|   |
 `--- ---`---`-------`--- ---`--- ---' `---' `---`-------`--- ---'
          _______ ___ ___ _______ _______ _______ ___ ___ 
         |   _   |   Y   |   _   |       |   _   |   Y   |
         |   1___|   1   |   1___|.|   | |.  1___|.      |
         |____   |\_   _/|____   `-|.  |-|.  __)_|. \_/  |
         |:  1   | |:  | |:  1   | |:  | |:  1   |:  |   |
         |::.. . | |::.| |::.. . | |::.| |::.. . |::.|:. |
         `-------' `---' `-------' `---' `-------`--- ---'"+
                              "\n\n\n"+
                              "                            _oo0oo_\n" +
                              "                           o8888888o\n" +
                              "                           88\" . \"88\n" +
                              "                           (| -_- |)\n" +
                              "                           0\\  =  /0\n" +
                              "                         ___/`---'\\___\n" +
                              "                       .' \\\\|     |// '.\n" +
                              "                      / \\\\|||  :  |||// \\ \n" +
                              "                     / _||||| -:- |||||- \\ \n" +
                              "                    |   | \\\\\\  -  /// |   |\n" +
                              "                    | \\_|  ''\\---/''  |_/ |\n" +
                              "                    \\  .-\\__  '-'  ___/-. /\n" +
                              "                  ___'. .'  /--.--\\  `. .'___\n" +
                              "               .\"\" '<  `.___\\_<|>_/___.' >' \"\".\n" +
                              "              | | :  `- \\`.;`\\ _ /`;.`/ - ` : | |\n" +
                              "              \\  \\ `_.   \\_ __\\ /__ _/   .-` /  /\n" +
                              "          =====`-.____`.___ \\_____/___.-`___.-'=====\n" +
                              "                               `=---='\n" +
                              "         \n" +
                              "         \n" +
                              "          ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
                              "        \n" +
                              "                   God Bless         No Bugs\n" +
                              "       \n");
            Title = "Data Migration System";
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppDomain.CurrentDomain.BaseDirectory}NLog.config");
            // Assigning ip address to a logger
            var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            var ip = host.AddressList.LastOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            LogManager.Configuration.Variables["sourceAddress"] = ip;
            
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