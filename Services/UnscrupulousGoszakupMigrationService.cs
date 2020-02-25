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
    /// migration of goszakup unscrupulous
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
            using var parsedUnscrupulousGoszakupContext = new ParsedUnscrupulousGoszakupContext();
            _total = parsedUnscrupulousGoszakupContext.UnscrupulousGoszakupDtos.Count();
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
            await using var parsedUnscrupulousGoszakupContext = new ParsedUnscrupulousGoszakupContext();

            foreach (var dto in parsedUnscrupulousGoszakupContext.UnscrupulousGoszakupDtos.Where(x =>
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

        private UnscrupulousGoszakup DtoToWeb(UnscrupulousGoszakupDto unscrupulousGoszakupDto)
        {
            var unscrupulousGoszakup = new UnscrupulousGoszakup();
            unscrupulousGoszakup.BiinCompanies = unscrupulousGoszakupDto.SupplierBiin;
            unscrupulousGoszakup.RelevanceDate = unscrupulousGoszakupDto.Relevance;
            unscrupulousGoszakup.Status = true;
            return unscrupulousGoszakup;
        }
    }
}