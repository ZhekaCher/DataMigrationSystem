using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
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


            Logger.Info("Starting removing participants who out of unscrupulous");
            await using var webUnscrupulousGoszakupContext = new WebUnscrupulousGoszakupContext();
            await using var webUnscrupulousGoszakupContext2 = new WebUnscrupulousGoszakupContext();
            await using var parsedUnscrupulousGoszakupContext = new ParsedUnscrupulousGoszakupContext();
            var firstParsedTime =
                await parsedUnscrupulousGoszakupContext.UnscrupulousGoszakupDtos.MinAsync(x => x.Relevance);
            var old = webUnscrupulousGoszakupContext.UnscrupulousGoszakup.Where(x =>
                x.RelevanceDate < firstParsedTime).Where(x => x.Status == true);
            var left = old.Count();
            foreach (var unscrupulousGoszakup in old)
                try
                {
                    unscrupulousGoszakup.Status = false;
                    unscrupulousGoszakup.RelevanceDate = firstParsedTime;
                    webUnscrupulousGoszakupContext2.UnscrupulousGoszakup.Update(unscrupulousGoszakup);
                    await webUnscrupulousGoszakupContext2.SaveChangesAsync();
                    Logger.Trace($"{unscrupulousGoszakup.BiinCompanies} Left: {--left}");
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                    Program.NumOfErrors++;
                }

            Logger.Info("Removing done");
            await parsedUnscrupulousGoszakupContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.unscrupulous_goszakup restart identity cascade;");
            Logger.Info("Truncated");
            // await webUnscrupulousGoszakupContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");
        }

        private async Task Migrate(int threadNum)
        {
            // Logger.Info("Started thread");


            await using var webUnscrupulousGoszakupContext = new WebUnscrupulousGoszakupContext();
            await using var parsedUnscrupulousGoszakupContext = new ParsedUnscrupulousGoszakupContext();

            foreach (var dto in parsedUnscrupulousGoszakupContext.UnscrupulousGoszakupDtos.Where(x =>
                    x.Pid % NumOfThreads == threadNum).Where(x => x.SupplierInnunp == null)
                .Include(x => x.RnuReferenceGoszakupDtos))
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
                        // Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
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

            // Logger.Info($"Completed thread at {_total}");
        }

        private UnscrupulousGoszakup DtoToWeb(UnscrupulousGoszakupDto unscrupulousGoszakupDto)
        {
            var unscrupulousGoszakup = new UnscrupulousGoszakup();
            unscrupulousGoszakup.BiinCompanies = unscrupulousGoszakupDto.SupplierBiin;
            unscrupulousGoszakup.RelevanceDate = unscrupulousGoszakupDto.Relevance;
            unscrupulousGoszakup.Status = true;
            if (unscrupulousGoszakupDto.RnuReferenceGoszakupDtos.Count > 0)
                unscrupulousGoszakup.StartDate =
                    unscrupulousGoszakupDto.RnuReferenceGoszakupDtos.OrderByDescending(x => x.StartDate).ToList()[0].StartDate;
            else
                unscrupulousGoszakup.StartDate = unscrupulousGoszakupDto.Relevance;
            return unscrupulousGoszakup;
        }
    }
}