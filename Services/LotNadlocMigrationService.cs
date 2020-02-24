using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class LotNadlocMigrationService : MigrationService
    {
        private readonly string _currentTradingFloor = "nadloc";
        private int _sTradingFloorId;
        private int _total;
        private object _lock = new object();

        public LotNadlocMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var ParsedLotNadlocContext = new ParsedLotNadlocContext();
            using var WebLotContext = new WebLotContext();
            _total = ParsedLotNadlocContext.NadlocLotsDtos.Count();
            _sTradingFloorId = WebLotContext.STradingFloors.FirstOrDefault(x => x.Code.Equals(_currentTradingFloor)).Id;
            
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
            Logger.Info("Started Thread");
            await using var webLotContext = new WebLotContext();
            await using var parsedLotNadlocContext = new ParsedLotNadlocContext();
            foreach (var dto in parsedLotNadlocContext.NadlocLotsDtos.Where(x=>x.Id % NumOfThreads == threadNum))
            {
                var dtoIns = LotNadlocDtoToLot(dto);
                dtoIns.IdTf = _sTradingFloorId;
                await webLotContext.Lots.Upsert(dtoIns).On(x => new {x.IdLot, x.IdTf}).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
        }

<<<<<<< HEAD
        private Lot LotNadlocDtoToLot(LotNadlocDto nadlocLotsDto)
        {
            var lot = new Lot();
            lot.IdLot = nadlocLotsDto.Id;
            lot.IdAnno = nadlocLotsDto.TenderId;
            lot.NumberLot = nadlocLotsDto.LotNumber.ToString();
            lot.NameRu = nadlocLotsDto.ScpDescription;
            lot.Quantity = nadlocLotsDto.Quantity;
            lot.Price = nadlocLotsDto.FullPrice/nadlocLotsDto.Quantity;
            lot.Total = nadlocLotsDto.FullPrice;
            lot.RelevanceDate = nadlocLotsDto.RelevanceDate;
            lot.DeliveryAddress = nadlocLotsDto.DeliveryPlace;

            return lot;
=======
        private Lot LotNadlocDtoToLot(NadlocLotsDto nadlocLotsDto)
        {
            var lot = new Lot();
<<<<<<< HEAD
            lot.Total = nadlocLotsDto.
>>>>>>> Tralala
=======
            // lot.Total = nadlocLotsDto.
            return lot;
>>>>>>> Lalala2
        }
    }
}