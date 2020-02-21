using System;
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
    /// INPUT
    /// </summary>
    public class LotsGoszakupMigrationService : MigrationService
    {
        private readonly ParsedLotGoszakupContext _parsedLotsGoszakupContext;
        private readonly WebLotContext _webLotsContext;
        private readonly string _currentTradingFloor = "goszakup";


        public LotsGoszakupMigrationService()
        {
            _parsedLotsGoszakupContext = new ParsedLotGoszakupContext();
            _webLotsContext = new WebLotContext();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            //TODO
            await Migrate();
        }
        
        private async Task Migrate()
        {
            Logger.Info("Started loading dtos");
            var sTradingFloorId = _webLotsContext.STradingFloors.FirstOrDefault(x => x.Code.Equals(_currentTradingFloor)).Id;
            var parsedLotsGoszakup = _parsedLotsGoszakupContext.LotGoszakupDtos.Take(25).ToList();
            Logger.Info("Loaded all dtos, starting migration...");
            foreach (var lotsGoszakupDto in parsedLotsGoszakup)
            {
                var lot = LotGoszakupDtoToLot(lotsGoszakupDto);
                lot.IdTf = sTradingFloorId;
                InsertOrUpdate(lot, _webLotsContext);
            }
            _webLotsContext.SaveChanges();
            Logger.Info("Completed loading");
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