using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class UnscrupulousSuppliersMigrationService : MigrationService
    {
        private readonly WebUnscrupulousSuppliersContext _webUnscrupulous;
        private readonly ParsedUnscrupulousSuppliers _parsedUnscrupulous;
        private readonly object _forLock;
        private int _counter;
        
        public UnscrupulousSuppliersMigrationService(int numOfThreads = 1)
        {
           _webUnscrupulous = new WebUnscrupulousSuppliersContext();
           _parsedUnscrupulous = new ParsedUnscrupulousSuppliers();
           NumOfThreads = numOfThreads;
           _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 1; i++)
            {
                tasks.Add(Migrate(i));
            }
            await Task.WhenAll(tasks);
            var parsedUnscrupulous = new ParsedUnscrupulousSuppliers();
            await parsedUnscrupulous.Database.ExecuteSqlRawAsync(
                "truncate avroradata.unscrupulous_suppliers restart identity;");

        }

        private async Task Migrate(int numThread)
        {
            await using var webUnscrupulous = new WebUnscrupulousSuppliersContext();
            await using var parsedUnscrupulous = new ParsedUnscrupulousSuppliers();
            var companiesDto = parsedUnscrupulous.CompanyBinDtos
                .Where(x => x.Code % NumOfThreads == numThread)
                .Include(x => x.UnscrupulousSuppliersDtos);
            foreach (var binDto in companiesDto)
            {
                if (binDto.UnscrupulousSuppliersDtos.Count ==0)
                    continue;
                var newlist = binDto.UnscrupulousSuppliersDtos.Select(x => new UnscrupulousSuppliers
                {
                    Name = x.Name,
                    SupplierAddress = x.SupplierAddress,
                    IinBiin = x.IinBiin,
                    BuyMethod = x.BuyMethod,
                    FinalDate = x.FinalDate,
                    TechName = x.TechName,
                    Court = x.Court,
                    RelevanceDate = DateTime.Now
                }).ToList();
                var oldList = webUnscrupulous.UnscrupulousSupplierses.Where(x => x.IinBiin == binDto.Code).ToList();
                webUnscrupulous.UnscrupulousSupplierses.RemoveRange(oldList);
                await webUnscrupulous.SaveChangesAsync();
                await webUnscrupulous.UnscrupulousSupplierses.AddRangeAsync(newlist);
                await webUnscrupulous.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}