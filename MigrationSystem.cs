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
        private Dictionary<ConfigurationElements, object> _configurations = PreloadConfigurations();
        private Dictionary<ConfigurationElements, object> _args;
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
            var listOfMigrations = ((List<string>) _configurations[ConfigurationElements.Migrations]).ToList();
            var threads = (int?) _configurations[ConfigurationElements.Threads];
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
                    if (e.InnerException is PostgresException)
                        _logger.Error($"Message:|{e.InnerException.Message}| at '{migration}'");
                    else if (e.InnerException != null)
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
                    _logger.Error($"Message:|{e.Message}; StackTrace:|{e.StackTrace}|");
                    Program.NumOfErrors++;
                }
            }
        }


        private static Dictionary<ConfigurationElements, object> PreloadConfigurations()
        {
            _commandsDictionary.Add("--help (-h)", "prints commands list");
            _commandsDictionary.Add("--list (-l)", "prints the list of avaliable migrations");
            _commandsDictionary.Add("--threads (-t)",
                $"choose number of threads\n{"Example: -t 5 -> starting with 5 threads for all migrations",82}");
            _commandsDictionary.Add("--migrations (-m)",
                $"allows to choose migrations\n{"Example: -m tax_debt wanted_individual -> starting the given migrations",94}");
            
            _helpString = _commandsDictionary.Aggregate("",
                (current, command) => current + $"{command.Key,-20} : {command.Value}\n");
            
            var conf = new Dictionary<ConfigurationElements, object>();
            conf.Add(ConfigurationElements.Threads, null);
            conf.Add(ConfigurationElements.Migrations, new List<string>()
            {
                "AnnouncementNadloc",
                "UnreliableTaxpayer"
            });
            return conf;
        }

        private static Dictionary<ConfigurationElements, object> ParseArguments(string[] args)
        {
            var arguments = new Dictionary<ConfigurationElements, object>();

            ConfigurationElements? flag = null;
            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                    flag = null;

                switch (flag)
                {
                    case ConfigurationElements.Threads:
                        int numOfThreads;
                        int.TryParse(arg, out numOfThreads);
                        if (numOfThreads <= 1 && numOfThreads >= 50)
                        {
                            _logger.Warn(
                                $"Unacceptable value for thread numbers '{arg}'; Value should correlate between 1 and 50 and match to the following form: '-t 5'");
                            Environment.Exit(1);
                        }

                        arguments[ConfigurationElements.Threads] = numOfThreads;
                        flag = null;
                        break;
                    case ConfigurationElements.Migrations:
                        var list = ((List<string>) arguments[ConfigurationElements.Migrations]).ToList();
                        list.Add(arg);
                        arguments[ConfigurationElements.Migrations] = list;
                        break;
                    case null:
                        switch (arg.ToLower())
                        {
                            case "-t":
                            case "--threads":
                                arguments.Add(ConfigurationElements.Threads, null);
                                flag = ConfigurationElements.Threads;
                                break;
                            case "-m":
                            case "--migrations":
                                arguments.Add(ConfigurationElements.Migrations, new List<string>());
                                flag = ConfigurationElements.Migrations;
                                break;
                            case "-h":
                            case "--help":
                                arguments.Add(ConfigurationElements.Help, null);
                                break;
                            case "-l":
                            case "--list":
                                arguments.Add(ConfigurationElements.List, null);
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
                if (_args.ContainsKey(ConfigurationElements.List))
                {
                    var migrations = ((List<string>) _configurations[ConfigurationElements.Migrations]).ToList();
                    Console.WriteLine("The list of avaliable migrations:");
                    foreach (var migration in migrations)
                        Console.WriteLine(migration);
                    Environment.Exit(0);
                }

                if (_args.ContainsKey(ConfigurationElements.Help))
                {
                    Console.Write(_helpString);
                    Environment.Exit(0);
                }

                switch (keyValuePair.Key)
                {
                    case ConfigurationElements.Threads:
                        _configurations[ConfigurationElements.Threads] = keyValuePair.Value ?? _configurations[ConfigurationElements.Threads];
                        break;
                    case ConfigurationElements.Migrations:
                        if (keyValuePair.Value != null)
                            _configurations[ConfigurationElements.Migrations] = ((List<string>) keyValuePair.Value).ToList();
                        break;
                }
            }
        }

        private static Type GetMigrationServiceFromName(string name)
        {
            return Type.GetType($"{typeof(MigrationService).Namespace}.{name}MigrationService");
        }
        
        private enum ConfigurationElements : byte
        {
            Threads,
            Migrations,
            Help,
            List
        }
    }
}