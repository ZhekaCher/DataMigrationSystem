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
        private readonly string _currentTradingFloor = "goszakup";
        private int _sTradingFloorId;
        private int _total;
        private object _lock = new object();


        public LotGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            using var webLotContext = new WebLotContext();
            _total = parsedLotGoszakupContext.LotGoszakupDtos.Count();
            _sTradingFloorId = webLotContext.STradingFloors
                .FirstOrDefault(x => x.Code.Equals(_currentTradingFloor)).Id;
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Warn(NumOfThreads);
            Logger.Info("Start");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("Ended");
        }
        
        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");

            await using var webLotContext = new WebLotContext();
            await using var parsedLotGoszakupContext = new ParsedLotGoszakupContext();
            foreach (var dto in parsedLotGoszakupContext.LotGoszakupDtos.Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                var dtoIns = LotGoszakupDtoToLot(dto);
                dtoIns.IdTf = _sTradingFloorId;
                await webLotContext.Lots.Upsert(dtoIns).On(x => new {x.IdLot, x.IdTf}).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
            
            Logger.Info("Completed thread");
        }

        private Lot LotGoszakupDtoToLot(LotGoszakupDto lotsGoszakupDto)
        {
            var lot = new Lot();
            lot.Total = lotsGoszakupDto.Amount;
            lot.Quantity = lotsGoszakupDto.Count;
            lot.CustomerBin = lotsGoszakupDto.CustomerBin;
            lot.DescriptionKz = lotsGoszakupDto.DescriptionKz;
            lot.DescriptionRu = lotsGoszakupDto.DescriptionRu;
            // lot.IdAnno = lotsGoszakupDto.TrdBuyNumberAnno ;
            lot.IdLot = lotsGoszakupDto.Id;
            lot.NameKz = lotsGoszakupDto.NameKz;
            lot.NameRu = lotsGoszakupDto.NameRu;
            lot.NumberLot = lotsGoszakupDto.LotNumber;
            lot.RelevanceDate = lotsGoszakupDto.Relevance;
            return lot;
        }

        private void InsertOrUpdate<T>(T entity, DbContext db) where T : class
        {
            if (db.Entry(entity).State == EntityState.Detached)
                db.Set<T>().Add(entity);
            db.SaveChanges(); 
        }
    }
}