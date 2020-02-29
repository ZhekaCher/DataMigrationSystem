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
        public UnreliableTaxpayerMigrationService(int numOfThreads = 10)
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
            webUnreliableTaxpayerContext.UnreliableTaxpayers.RemoveRange(webUnreliableTaxpayerContext.UnreliableTaxpayers.Where(x=>x.RelevanceDate<minDate));
            await webUnreliableTaxpayerContext.SaveChangesAsync();
            await parsedUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync("truncate table avroradata.unreliable_taxpayers;");
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            
            var taxpayers = from unreliableTaxpayerDto in parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos
                join companyBinDto in parsedUnreliableTaxpayerContext.CompanyBinDtos
                    on unreliableTaxpayerDto.BinCompany equals companyBinDto.Code
                    where unreliableTaxpayerDto.Id % NumOfThreads == numThread
                select new UnreliableTaxpayer
                {
                    RelevanceDate = unreliableTaxpayerDto.RelevanceDate,
                    DocumentDate = unreliableTaxpayerDto.DocumentDate,
                    IdListType = unreliableTaxpayerDto.IdListType,
                    IdTypeDocument = unreliableTaxpayerDto.IdTypeDocument,
                    Note = unreliableTaxpayerDto.Note,
                    DocumentNumber = unreliableTaxpayerDto.DocumentNumber,
                    BinCompany = unreliableTaxpayerDto.BinCompany
                };
            foreach (var taxpayer in taxpayers)
            {
                await webUnreliableTaxpayerContext.UnreliableTaxpayers.Upsert(taxpayer).On(x => new {x.BinCompany, x.IdListType}).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
    }
}