using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class Top100GoszakupMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter = 0;
        public Top100GoszakupMigrationService(int numOfThreads = 1)
        {

            _forLock = new object();
            
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webTop100GoszakupContext = new WebTop100GoszakupContext();
            await using var parsedTop100GoszakupContext = new ParsedTop100GoszakupContext();
            var top100Customers = parsedTop100GoszakupContext.Top100CustomersGoszakupDtos.Select(x =>
                new Top100CustomersGoszakup
                {
                    Contracts  = x.Contracts,
                    Amount = x.Amount,
                    AddingDate = x.AddingDate,
                    Bin = x.Bin,
                    Place = x.Place
                });
            foreach (var top100 in top100Customers)
            {
                await webTop100GoszakupContext.Top100CustomersGoszakup.Upsert(top100).On(x => x.Bin).RunAsync();
            }
            var top100Suppliers = parsedTop100GoszakupContext.Top100SuppliersgoszakupDtos.Select(x =>
                new Top100SuppliersGoszakup
                {
                    Contracts  = x.Contracts,
                    Amount = x.Amount,
                    AddingDate = x.AddingDate,
                    Bin = x.Bin,
                    Place = x.Place
                });
            foreach (var top100 in top100Suppliers)
            {
                await webTop100GoszakupContext.Top100Suppliersgoszakup.Upsert(top100).On(x => x.Bin).RunAsync();
            }
        }
    }
}