using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 11:08:36
    /// @version 1.0
    /// <summary>
    /// migration of lots
    /// </summary>
    public class LotGoszakupMigrationService : MigrationService
    {
        private const string CurrentTradingFloor = "goszakup";
        private readonly int _sTradingFloorId;
        private int _total;
        private readonly object _lock = new object();


        public LotGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            using var webLotContext = new WebLotContext();
            _total = parsedLotGoszakupContext.LotGoszakupDtos.Count();
            _sTradingFloorId = webLotContext.STradingFloors
                .FirstOrDefault(x => x.Code.Equals(CurrentTradingFloor)).Id;
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
            await using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            // await parsedLotGoszakupContext.Database.ExecuteSqlRawAsync("truncate table avroradata.lot_goszakup restart identity cascade;");
            Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            // Logger.Info("Started thread");

            await using var webLotContext = new WebLotContext();
            await using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            
                var parsedLotGoszakupDtos =
                    parsedLotGoszakupContext.LotGoszakupDtos.Where(x => x.Id % NumOfThreads == threadNum);
                foreach (var dto in parsedLotGoszakupDtos)
                {
                    var dtoIns = DtoToWeb(dto);
                    dtoIns.IdTf = _sTradingFloorId;
                    try
                    {
                        await webLotContext.Lots.Upsert(dtoIns).On(x => new {x.IdLot, x.IdTf}).RunAsync();
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("violates foreign key"))
                        {
                            // Logger.Warn($"Message:|{e.Message}|; IdContract:|{temp.IdContract}|;");
                        }
                        else
                        {
                            Logger.Error(
                                $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; IdLot:|{dtoIns.IdLot}|; Id:|{dtoIns.Id}|");
                            Program.NumOfErrors++;
                        }
                    }

                    lock (_lock)
                        Logger.Trace($"Left {--_total}");
                }
            

            Logger.Info($"Completed thread at {_total}");
        }

        private static Lot DtoToWeb(LotGoszakupDto lotGoszakupDto)
        {
            var lot = new Lot();
            lot.Total = lotGoszakupDto.Amount;
            lot.Quantity = lotGoszakupDto.Count;
            lot.CustomerBin = lotGoszakupDto.CustomerBin;
            lot.DescriptionKz = lotGoszakupDto.DescriptionKz;
            lot.DescriptionRu = lotGoszakupDto.DescriptionRu;
            if (lotGoszakupDto.TrdBuyId != null) lot.IdAnno = (long) lotGoszakupDto.TrdBuyId;
            lot.IdLot = lotGoszakupDto.Id;
            lot.NameKz = lotGoszakupDto.NameKz;
            lot.NameRu = lotGoszakupDto.NameRu;
            lot.NumberLot = lotGoszakupDto.LotNumber;
            lot.RelevanceDate = lotGoszakupDto.Relevance;
            return lot;
        }
    }
}