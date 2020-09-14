using System;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class Top100GoszakupMigrationService : MigrationService
    {
        private readonly object _forLock;
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
            var top100CustomersDto = from top100Customer in parsedTop100GoszakupContext.Top100CustomersGoszakupDtos
                join company in parsedTop100GoszakupContext.CompanyBinDtos
                    on top100Customer.Bin equals company.Code where top100Customer.Bin != null
                select
                    new Top100CustomersGoszakup
                    {
                        Contracts = top100Customer.Contracts,
                        Amount = top100Customer.Amount,
                        AddingDate = top100Customer.AddingDate,
                        Bin = top100Customer.Bin,
                        Place = top100Customer.Place
                    };
            foreach (var top100 in top100CustomersDto)
            {
                if (top100.Bin == null)
                {
                    Console.WriteLine("Hello");
                }
                await webTop100GoszakupContext.Top100CustomersGoszakup.Upsert(top100).On(x => x.Bin).RunAsync();
            }
            var lastDateC = webTop100GoszakupContext.Top100CustomersGoszakup.Max(x => x.AddingDate).Date;
            webTop100GoszakupContext.Top100CustomersGoszakup.RemoveRange(webTop100GoszakupContext.Top100CustomersGoszakup.Where(x=>x.AddingDate.Date < lastDateC));
            await webTop100GoszakupContext.SaveChangesAsync();

            var top100SuppliersDto = from top100Supplier in parsedTop100GoszakupContext.Top100SuppliersgoszakupDtos
                join company in parsedTop100GoszakupContext.CompanyBinDtos
                    on top100Supplier.Bin equals company.Code
                select
                    new Top100SuppliersGoszakup
                    {
                        Contracts = top100Supplier.Contracts,
                        Amount = top100Supplier.Amount,
                        AddingDate = top100Supplier.AddingDate,
                        Bin = top100Supplier.Bin,
                        Place = top100Supplier.Place
                    };
            foreach (var top100 in top100SuppliersDto)
            {
                await webTop100GoszakupContext.Top100Suppliersgoszakup.Upsert(top100).On(x => x.Bin).RunAsync();
            }
            var lastDateS = webTop100GoszakupContext.Top100Suppliersgoszakup.Max(x => x.AddingDate).Date;
            webTop100GoszakupContext.Top100Suppliersgoszakup.RemoveRange(webTop100GoszakupContext.Top100Suppliersgoszakup.Where(x=>x.AddingDate.Date < lastDateS));
            await webTop100GoszakupContext.SaveChangesAsync();
            await parsedTop100GoszakupContext.Database.ExecuteSqlRawAsync("truncate avroradata.top100customers, avroradata.top100suppliers;");
        }
    }
}