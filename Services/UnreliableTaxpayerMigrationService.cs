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
            
            var taxpayers = parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos
                .Join(parsedUnreliableTaxpayerContext.CompanyBinDtos,
                    unreliableTaxpayerDto => unreliableTaxpayerDto.BinCompany, companyBinDto => companyBinDto.Code,
                    (unreliableTaxpayerDto, companyBinDto) => new {unreliableTaxpayerDto, companyBinDto})
                .Where(t => t.unreliableTaxpayerDto.Id % NumOfThreads == numThread)
                .Select(t => new UnreliableTaxpayer
                {
                    RelevanceDate = t.unreliableTaxpayerDto.RelevanceDate,
                    DocumentDate = t.unreliableTaxpayerDto.DocumentDate,
                    IdListType = t.unreliableTaxpayerDto.IdListType,
                    IdTypeDocument = t.unreliableTaxpayerDto.IdTypeDocument,
                    Note = t.unreliableTaxpayerDto.Note,
                    DocumentNumber = t.unreliableTaxpayerDto.DocumentNumber,
                    BinCompany = t.unreliableTaxpayerDto.BinCompany
                });
            await webUnreliableTaxpayerContext.UnreliableTaxpayers.UpsertRange(taxpayers).On(x => new {x.BinCompany, x.IdListType}).RunAsync();
        }
    }
}