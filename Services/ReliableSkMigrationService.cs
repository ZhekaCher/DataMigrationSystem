using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class ReliableSkMigrationService : MigrationService
    {
        private readonly WebReliableSkContext _webReliableSkContext;
        private readonly ParsedReliableSkContext _parsedReliableSkContext;

        public ReliableSkMigrationService(int numOfThreats = 1)
        {
            NumOfThreads = numOfThreats;
            _webReliableSkContext = new WebReliableSkContext();
            _parsedReliableSkContext = new ParsedReliableSkContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override  async Task StartMigratingAsync()
        {
            var reliableSkDtos = from reliableSkDto in _parsedReliableSkContext.ReliableSkDtos
                join company in _parsedReliableSkContext.CompanyBinDtos
                    on reliableSkDto.Bin equals company.Code
                    select new ReliableSk
                {
                    Reason = reliableSkDto.Reason,
                    AddingDate = reliableSkDto.AddingDate,
                    RelevanceDate = reliableSkDto.RelevanceDate,
                    Biin = reliableSkDto.Bin
                };
            foreach (var reliableSkDto in reliableSkDtos)
            {
                await _webReliableSkContext.ReliableSks.Upsert(reliableSkDto).On(x => x.Biin).RunAsync();
            }
            var lastDate = _webReliableSkContext.ReliableSks.Max(x => x.RelevanceDate).Date;
            _webReliableSkContext.ReliableSks.RemoveRange(_webReliableSkContext.ReliableSks.Where(x=>x.RelevanceDate<lastDate));
            await _webReliableSkContext.SaveChangesAsync();
            await _parsedReliableSkContext.Database.ExecuteSqlRawAsync("truncate avroradata.reliable_suppliers_sk restart identity;");
        }
    }
}