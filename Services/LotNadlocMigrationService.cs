using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.TradingFloor;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class LotNadlocMigrationService : MigrationService
    {
        private readonly string _currentTradingFloor = "nedro";
        private int _sTradingFloorId;
        private int _total;
        private readonly object _lock = new object();

        public LotNadlocMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedLotNadlocContext = new ParsedLotNadlocContext();
            using var webLotContext = new WebLotContext();
            _total = parsedLotNadlocContext.NadlocLotsDtos.Count();
            _sTradingFloorId = webLotContext.STradingFloors.FirstOrDefault(x => x.Code.Equals(_currentTradingFloor)).Id;

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
            await using var parsedLotNadlocContext = new ParsedLotNadlocContext();
            await parsedLotNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_lots");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var webLotContext = new WebLotContext();
            await using var parsedLotNadlocContext = new ParsedLotNadlocContext();
            var lotDtos =  parsedLotNadlocContext.NadlocLotsDtos.Where(x => x.TenderId % NumOfThreads == threadNum).OrderBy(x=>x.TenderId)
                .Select(x => new Lot
                {
                    IdTf = _sTradingFloorId,
                    IdLot = x.Id,
                    IdAnno = x.TenderId,
                    NumberLot = x.LotNumber.ToString(),
                    NameRu = x.ScpDescription,
                    Quantity = x.Quantity == 0 ? 1 : x.Quantity,
                    Price = x.FullPrice/(x.Quantity == 0 ? 1 : x.Quantity),
                    Total = x.FullPrice,
                    RelevanceDate = x.RelevanceDate,
                    DeliveryAddress = x.DeliveryPlace
                });
            long tenderId = 0;
            
            foreach (var dto in lotDtos)
            {
                if (tenderId != dto.IdAnno)
                {
                    tenderId = dto.IdAnno;
                    webLotContext.Lots.RemoveRange(webLotContext.Lots.Where(x=>x.IdAnno==dto.IdAnno));
                    await webLotContext.SaveChangesAsync();

                }

                await webLotContext.Lots.AddAsync(dto);
                await webLotContext.SaveChangesAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }
        }
    }
}