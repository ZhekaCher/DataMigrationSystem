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
    public class UnreliableTaxpayerMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter;
        public UnreliableTaxpayerMigrationService(int numOfThreads = 1)
        {
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
            for (var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }

            await Task.WhenAll(tasks);
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            var minDate = await parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.MinAsync(x => x.RelevanceDate);
            await webUnreliableTaxpayerContext.Database.ExecuteSqlInterpolatedAsync(
                $"delete from avroradata.lists where relevance_date<{minDate}");
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            var taxpayers =
                parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.Where(x => x.Id % NumOfThreads == numThread).Select(x=>new UnreliableTaxpayer
                {
                    RelevanceDate = x.RelevanceDate,
                    DocumentDate = x.DocumentDate,
                    IdListType = x.IdListType,
                    IdTypeDocument = x.IdTypeDocument,
                    Note = x.Note,
                    DocumentNumber = x.DocumentNumber,
                    BinCompany = x.BinCompany
                });
            foreach (var taxpayer in taxpayers)
            {
                await webUnreliableTaxpayerContext.UnreliableTaxpayers.Upsert(taxpayer).On(x => x.BinCompany).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
    }
}