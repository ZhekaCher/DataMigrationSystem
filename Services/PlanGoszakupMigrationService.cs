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
        
        public PlanGoszakupMigrationService(int numOfThreads = 1)
        {
            using var parsedGoszakupContext = new ParsedGoszakupContext();
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
            await using var parsedGoszakupContext = new ParsedGoszakupContext();
         
            foreach (var dto in parsedGoszakupContext.PlanGoszakupDtos.Where(x =>
                    x.Id % NumOfThreads == threadNum))
            {
                try
                {
                    await using var innerParsedGoszakupContext = new ParsedGoszakupContext();
                    await using var adataTenderContext = new AdataTenderContext();
                    var lot = innerParsedGoszakupContext.LotGoszakupDtos.FirstOrDefault(x => x.PlanPoint == dto.Id);
                    if (lot != null)
                    {
                        var webLot = adataTenderContext.AdataLots.FirstOrDefault(x => x.SourceNumber == lot.LotNumber);
                        if (webLot != null)
                        {
                            webLot.TruCode = dto.RefEnstruCode;
                            // if (webLot != null) webLot.Title = dto.NameRu;
                            webLot.Terms = dto.SupplyDateRu;
                            adataTenderContext.Update(webLot);
                        }

                        await adataTenderContext.SaveChangesAsync();
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