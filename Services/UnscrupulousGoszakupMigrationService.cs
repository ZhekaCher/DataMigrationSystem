using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:09:59
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class UnscrupulousGoszakupMigrationService : MigrationService
    {
        private int _total;
        private object _lock = new object();

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public UnscrupulousGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedUnscrupulousesGoszakupContext = new ParsedUnscrupulousesGoszakupContext();
            _total = parsedUnscrupulousesGoszakupContext.UnscrupulousesGoszakupDtos.Count();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");


            await using var webUnscrupulousGoszakupContext = new WebUnscrupulousGoszakupContext();
            await using var parsedUnscrupulousesGoszakupContext = new ParsedUnscrupulousesGoszakupContext();

            foreach (var dto in parsedUnscrupulousesGoszakupContext.UnscrupulousesGoszakupDtos.Where(x =>
                x.Pid % NumOfThreads == threadNum).Where(x => x.SupplierInnunp == null))
            {
                var temp = DtoToWeb(dto);
                try
                {
                    await webUnscrupulousGoszakupContext.UnscrupulousGoszakup.Upsert(temp).On(x => x.BiinCompanies)
                        .RunAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
                    }
                    else
                    {
                        Logger.Error(
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; Biin:|{temp.BiinCompanies}|;");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            Logger.Info("Completed thread");
        }

        private UnscrupulousGoszakup DtoToWeb(UnscrupulousesGoszakupDto unscrupulousesGoszakupDto)
        {
            var unscrupulousGoszakup = new UnscrupulousGoszakup();
            unscrupulousGoszakup.BiinCompanies = unscrupulousesGoszakupDto.SupplierBiin;
            unscrupulousGoszakup.RelevanceDate = unscrupulousesGoszakupDto.Relevance;
            unscrupulousGoszakup.Status = true;
            return unscrupulousGoszakup;
        }
    }
}