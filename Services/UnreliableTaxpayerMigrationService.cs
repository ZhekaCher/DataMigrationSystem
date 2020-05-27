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
                $"delete from avroradata.unreliable_taxpayers where relevance_date<{minDate}");
            await parsedUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync("truncate table avroradata.unreliable_taxpayers;");
            await webUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            
            var taxpayers = from dto in parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos
                where dto.Id % NumOfThreads == numThread
                join com in parsedUnreliableTaxpayerContext.CompanyBinDtos on dto.BinCompany equals com.Code
                select new UnreliableTaxpayer
                {
                    RelevanceDate = dto.RelevanceDate,
                    DocumentDate = dto.DocumentDate,
                    IdListType = dto.IdListType,
                    IdTypeDocument = dto.IdTypeDocument,
                    Note = dto.Note,
                    DocumentNumber = dto.DocumentNumber,
                    BinCompany = dto.BinCompany
                };
            await webUnreliableTaxpayerContext.UnreliableTaxpayers.UpsertRange(taxpayers).On(x => new {x.BinCompany, x.IdListType}).RunAsync();
        }
    }
}