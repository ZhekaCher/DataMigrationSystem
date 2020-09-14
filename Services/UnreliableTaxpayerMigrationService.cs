using System.Collections.Generic;
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
    public class UnreliableTaxpayerMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _count;
        public UnreliableTaxpayerMigrationService(int numOfThreads = 20)
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
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            _count = await parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.CountAsync();
            await MigrateAsync();
            
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            var minDate = await parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.MinAsync(x => x.RelevanceDate);
            await webUnreliableTaxpayerContext.Database.ExecuteSqlInterpolatedAsync($"delete from avroradata.unreliable_taxpayers where relevance_date<{minDate}");
            await parsedUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync("truncate table avroradata.unreliable_taxpayers;");
            await webUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");
        }

        private async Task Insert(UnreliableTaxpayer unreliableTaxpayer)
        {
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            await webUnreliableTaxpayerContext.UnreliableTaxpayers.Upsert(unreliableTaxpayer).On(x => new {x.BinCompany, x.IdListType}).RunAsync();
            lock (_forLock)
            {
                Logger.Trace(--_count);
            }
        }
        private async Task MigrateAsync()
        {
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            var taxpayers = parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos
                .Join(parsedUnreliableTaxpayerContext.CompanyBinDtos, dto => dto.BinCompany, com => com.Code,
                    (dto, com) => new UnreliableTaxpayer
                    {
                        RelevanceDate = dto.RelevanceDate,
                        DocumentDate = dto.DocumentDate,
                        IdListType = dto.IdListType,
                        IdTypeDocument = dto.IdTypeDocument,
                        Note = dto.Note,
                        DocumentNumber = dto.DocumentNumber,
                        BinCompany = dto.BinCompany
                    });
            var tasks = new List<Task>();
            foreach (var taxpayer in taxpayers)
            {
                tasks.Add(Insert(taxpayer));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}