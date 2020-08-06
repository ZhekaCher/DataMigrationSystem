using System;
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
            using var parsedGoszakupContext = new ParsedGoszakupContext();
            parsedGoszakupContext.Database.ExecuteSqlRaw(
                "delete from avroradata.plan_goszakup p where p.id not in (select plan_point from avroradata.lot_goszakup)");
            _total = parsedGoszakupContext.PlanGoszakupDtos.Count();
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
        }

        private async Task Migrate(int threadNum)
        {
            await Task.Delay(1);
            await using var parsedGoszakupContext = new ParsedGoszakupContext();
            foreach (var dto in parsedGoszakupContext.PlanGoszakupDtos.Where(x =>
                    x.Id % NumOfThreads == threadNum))
            {
                try
                {
                    if (dto.Id==35959475)
                    {
                        Console.WriteLine(2); 
                    }
                    await using var innerParsedGoszakupContext = new ParsedGoszakupContext();
                    await using var adataTenderContext = new AdataTenderContext();
                    var lot = innerParsedGoszakupContext.LotGoszakupDtos.FirstOrDefault(x => x.PlanPoint == dto.Id);
                    if (lot.LotNumber.Contains("35332956-ОК1"))
                    {
                        Console.WriteLine(1);
                    }
                    if (lot != null)
                    {
                        adataTenderContext.Database.ExecuteSqlRawAsync($"UPDATE lots SET tru_code = '{dto.RefEnstruCode}', terms= '{dto.SupplyDateRu}' WHERE source_number = '{lot.LotNumber}'").GetAwaiter().GetResult();
                    }
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