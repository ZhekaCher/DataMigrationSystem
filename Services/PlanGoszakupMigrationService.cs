﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Context.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class PlanGoszakupMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();

        public PlanGoszakupMigrationService(int numOfThreads = 20)
        {
            using var adataTenderContext = new AdataTenderContext();
            // using var parsedGoszakupContext = new ParsedGoszakupContext();
            // parsedGoszakupContext.Database.ExecuteSqlRaw(
            // "delete from avroradata.plan_goszakup p where p.id not in (select plan_point from avroradata.lot_goszakup)");
            _total = adataTenderContext.AdataLots.Count(x => x.SourceId == 2 && x.TruCode == null);
            adataTenderContext.Database.ExecuteSqlRaw("UPDATE adata_tender.announcements SET status_id=58 WHERE application_finish_date<now()");
            adataTenderContext.Database.ExecuteSqlRaw("UPDATE adata_tender.lots SET status_id=38 WHERE application_finish_date<now()");
            adataTenderContext.Dispose();
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            await using var parsedGoszakupContext = new ParsedGoszakupContext();
            parsedGoszakupContext.Database.ExecuteSqlRaw("truncate table avroradata.announcement_goszakup restart identity cascade;");
            parsedGoszakupContext.Database.ExecuteSqlRaw("truncate table avroradata.lot_goszakup restart identity cascade;");
            await parsedGoszakupContext.DisposeAsync();
        }

        private async Task Migrate(int threadNum)
        {
            await Task.Delay(1);
            await using var adataTenderContext = new AdataTenderContext();

            foreach (var model in adataTenderContext.AdataLots.Where(x =>
                x.Id % NumOfThreads == threadNum &&
                x.SourceId == 2 &&
                x.TruCode == null))
            {
                try
                {
                    await using var tempAdataTenderContext = new AdataTenderContext();
                    await using var innerParsedGoszakupContext = new ParsedGoszakupContext();
                    var dtoLot =
                        innerParsedGoszakupContext.LotGoszakupDtos.FirstOrDefault(
                            x => x.LotNumber == model.SourceNumber);
                    if (dtoLot == null)
                        continue;
                    var plan = innerParsedGoszakupContext.PlanGoszakupDtos.FirstOrDefault(x =>
                        x.Id == dtoLot.PlanPoint);
                    if (plan != null)
                    {
                        tempAdataTenderContext.Database
                            .ExecuteSqlRawAsync(
                                $"UPDATE lots SET tru_code = '{plan.RefEnstruCode}', terms= '{plan.SupplyDateRu}' WHERE source_number = '{model.SourceNumber}'")
                            .GetAwaiter().GetResult();
                    }

                    innerParsedGoszakupContext.Dispose();
                    tempAdataTenderContext.Dispose();
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
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
        }
    }
}