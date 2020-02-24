using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
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


        public LotGoszakupMigrationService(int numOfThreads = 1)
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
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");

            await using var webLotContext = new WebLotContext();
            await using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            await using var parsedAnnouncementGoszakupContext = new ParsedAnnouncementGoszakupContext();
            foreach (var dto in parsedLotGoszakupContext.LotGoszakupDtos.Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                var dtoIns = LotGoszakupDtoToLot(dto);
                dtoIns.IdTf = _sTradingFloorId;
                try
                {
                    await webLotContext.Lots.Upsert(dtoIns).On(x => new {x.IdLot, x.IdTf}).RunAsync();
                }
                catch (Exception e)
                {
                    Logger.Warn(e);
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }

            Logger.Info("Completed thread");
        }

        private static Lot LotGoszakupDtoToLot(LotGoszakupDto lotsGoszakupDto)
        {
            var lot = new Lot();
            lot.Total = lotsGoszakupDto.Amount;
            lot.Quantity = lotsGoszakupDto.Count;
            lot.CustomerBin = lotsGoszakupDto.CustomerBin;
            lot.DescriptionKz = lotsGoszakupDto.DescriptionKz;
            lot.DescriptionRu = lotsGoszakupDto.DescriptionRu;
            lot.IdAnno = 3308910;
            if (lotsGoszakupDto.TrdBuyId != null) lot.IdAnno = (long) lotsGoszakupDto.TrdBuyId;
            lot.IdLot = lotsGoszakupDto.Id;
            lot.NameKz = lotsGoszakupDto.NameKz;
            lot.NameRu = lotsGoszakupDto.NameRu;
            lot.NumberLot = lotsGoszakupDto.LotNumber;
            lot.RelevanceDate = lotsGoszakupDto.Relevance;
            return lot;
        }
    }
}