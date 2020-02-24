using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using DataMigrationSystem.Services;
using NLog;
using Npgsql;
// ReSharper disable CognitiveComplexity

namespace DataMigrationSystem
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 13:34:45
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class MigrationSystem
    {
        private static Logger _logger;
        private Dictionary<string, object> _configurations = PreloadConfigurations();
        private Dictionary<string, object> _args;
        private static Dictionary<string, string> _commandsDictionary = new Dictionary<string, string>();
        private static string _helpString;

        internal MigrationSystem(string[] args)
        {
            _logger = LogManager.GetCurrentClassLogger();

            _args = ParseArguments(args);
            ProceedArguments();
        }

        public async Task StartMigrations()
        {
            var listOfMigrations = ((List<string>) _configurations["migrations"]).ToList();
            var threads = (int?) _configurations["threads"];
            foreach (var migration in listOfMigrations)
            {
                MigrationService migrationService;
                var migrationClass = GetMigrationServiceFromName(migration);
                _logger.Info($"Trying to launch {migration}");
                try
                {
                    if (threads == null)
                    {
                        migrationService =
                            (MigrationService) Activator.CreateInstance(migrationClass,
                                (int) migrationClass.GetConstructors()[0].GetParameters()[0].DefaultValue);
                    }
                    else
                    {
                        migrationService =
                            (MigrationService) Activator.CreateInstance(migrationClass,
                                threads);
                    }

                    await migrationService.StartMigratingAsync();
                }
                catch (TargetInvocationException e)
                {
                    if (e.InnerException.GetType() == typeof(PostgresException))
                        _logger.Error($"Message:|{e.InnerException.Message}| at '{migration}'");
                    else
                        _logger.Error(
                            $"Message:|{e.InnerException.Message}; StackTrace:|{e.InnerException.StackTrace}|");
                    Program.NumOfErrors++;
                }
                catch (IndexOutOfRangeException)
                {
                    _logger.Error(
                        $"Try to implement Constructor: |MigrationService(int numOfThreads = 1)| in {migration} class");
                    Program.NumOfErrors++;
                }
                catch (NullReferenceException)
                {
                    _logger.Error($"It seems that '{migration}' doesn't exist");
                    Program.NumOfErrors++;
                }
                catch (Exception e)
                {
                    _logger.Error($"Message:|{e.InnerException.Message}; StackTrace:|{e.InnerException.StackTrace}|");
                    Program.NumOfErrors++;
                }
            }
        }


        private static Dictionary<string, object> PreloadConfigurations()
        {
            _commandsDictionary.Add("--help (-h)", "prints commands list");
            _commandsDictionary.Add("--list (-l)", "prints the list of avaliable migrations");
            _commandsDictionary.Add("--threads (-t)",
                $"choose number of threads\n{"Example: -t 5 -> starting with 5 threads for all migrations",82}");
            _commandsDictionary.Add("--migrations (-m)",
                $"allows to choose migrations\n{"Example: -m tax_debt wanted_individual -> starting the given migrations",94}");
            
            _helpString = _commandsDictionary.Aggregate("",
                (current, command) => current + $"{command.Key,-20} : {command.Value}\n");
            
            var conf = new Dictionary<string, object>();
            conf.Add("threads", null);
            conf.Add("migrations", new List<string>()
            {
                "LotGoszakupMigrationService"
            });
            return conf;
        }

        private Dictionary<string, object> ParseArguments(string[] args)
        {
            //TODO(Rewrite Id Using enumerators)
            var arguments = new Dictionary<string, object>();

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
                            _logger.Warn(
                                $"Unacceptable value for thread numbers '{arg}'; Value should correlate between 1 and 50 and match to the following form: '-t 5'");
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
                            case "-h":
                            case "--help":
                                arguments.Add("help", null);
                                break;
                            case "-l":
                            case "--list":
                                arguments.Add("list", null);
                                break;
                            default:
                                _logger.Warn($"Found unknown argument '{arg}'; Check if your arguments are correct");
                                Console.WriteLine(_helpString);
                                Environment.Exit(1);
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
                if (_args.ContainsKey("list"))
                {
                    var migrations = ((List<string>) _configurations["migrations"]).ToList();
                    Console.WriteLine("The list of avaliable migrations:");
                    foreach (var migration in migrations)
                        Console.WriteLine(migration);
                    Environment.Exit(0);
                }

                if (_args.ContainsKey("help"))
                {
                    Console.Write(_helpString);
                    Environment.Exit(0);
                }

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
        }

        private static Type GetMigrationServiceFromName(string name)
        {
            return Type.GetType($"{typeof(MigrationService).Namespace}.{name}");
        }
    }
}