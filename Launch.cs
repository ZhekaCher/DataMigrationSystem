using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DataMigrationSystem.Services;
using NLog;

namespace DataMigrationSystem
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 13:34:45
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class Launch
    {
        private static Logger logger;
        private Dictionary<string, object> _configurations = PreloadConfigurations();
        private Dictionary<string, object> _args;
        private List<MigrationService> _migrations = new List<MigrationService>();

        internal Launch(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();
            _args = ParseArguments(args);
            ProceedArguments();
        }


        private static Dictionary<string, object> PreloadConfigurations()
        {
            var conf = new Dictionary<string, object>();

            conf.Add("threads", null);
            conf.Add("migrations", null);
            return conf;
        }

        private Dictionary<string, object> ParseArguments(string[] args)
        {
            //TODO(Rewrite Id Using enumerators)
            var arguments = new Dictionary<string, object>();
            arguments.Add("list", null);
            arguments.Add("help", null);

            string flag = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                    flag = null;

                switch (flag)
                {
                    case "threads":
                        int numOfThreads;
                        int.TryParse(arg, out numOfThreads);
                        if (numOfThreads <= 1 && numOfThreads >= 50)
                        {
                            logger.Warn(
                                $"Unacceptable value for thread numbers '{arg}'; Value should correlate between 1 and 50 and match to the following form: '-t5'");
                            Environment.Exit(1);
                        }

                        arguments["threads"] = numOfThreads;
                        flag = null;
                        break;
                    case "migrations":
                        var list = ((List<string>) arguments["migrations"]).ToList();
                        list.Add(arg);
                        arguments["migrations"] = list;
                        break;
                    case null:
                        switch (arg.ToLower())
                        {
                            case "-t":
                            case "--threads":
                                arguments.Add("threads", null);
                                flag = "threads";
                                break;
                            case "-m":
                            case "--migrations":
                                arguments.Add("migrations", new List<string>());
                                flag = "migrations";
                                break;
                            default:
                                break;
                        }

                        break;
                    default:
                        break;
                }
            }

            return arguments;
        }

        private void ProceedArguments()
        {
            foreach (var keyValuePair in _args)
            {
                switch (keyValuePair.Key)
                {
                    case "threads":
                        _configurations["threads"] = keyValuePair.Value ?? _configurations["threads"];
                        break;
                    case "migrations":
                        if (keyValuePair.Value != null)
                            _configurations["migrations"] = ((List<string>) keyValuePair.Value).ToList();
                        break;
                }
            }
        // }
        //
        //
        // /// @author Yevgeniy Cherdantsev
        // /// @date 24.02.2020 13:16:53
        // /// @version 1.0
        // /// <summary>
        // /// INPUT
        // /// </summary>
        // /// <param name="migrationList"></param>
        // private static async Task Migrate()
        // {
        //     if (migrationList == null)
        //         foreach (var keyValuePair in _migrations)
        //         {
        //             try
        //             {
        //                 await keyValuePair.Value.StartMigratingAsync();
        //             }
        //             catch (Exception e)
        //             {
        //                 Program.NumOfErrors++;
        //                 logger.Error($"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
        //             }
        //         }
        //     else
        //         foreach (var migration in migrationList)
        //             try
        //             {
        //                 await _migrations[migration].StartMigratingAsync();
        //             }
        //             catch (Exception e)
        //             {
        //                 Program.NumOfErrors++;
        //                 logger.Error($"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
        //             }
         }

        private Type GetMigrationServiceFromName(string name)
        {
            return Type.GetType($"DataMigrationSystem.Services.{name}");
        }
    }
}