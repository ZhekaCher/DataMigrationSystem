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
    public class UnreliableSkMigrationService:MigrationService
    {
        private readonly WebUnreliableSkContext _webUnreliableSkContext;
        private readonly ParsedUnreliableSkContext _parsedUnreliableSkContext;

        public UnreliableSkMigrationService(int numOfThreads=1)
        {
            _webUnreliableSkContext = new WebUnreliableSkContext();
            _parsedUnreliableSkContext = new ParsedUnreliableSkContext();
        }

        public override async Task StartMigratingAsync()
        {
            var unreliableSkDtos = from unreliableSkDto in _parsedUnreliableSkContext.UnreliableSkDtos 
                join company in _parsedUnreliableSkContext.CompanyBinDtos
                on unreliableSkDto.Bin equals company.Code
                select new UnreliableSk
                {
                    Reason = unreliableSkDto.Reason,
                    AddingDate = unreliableSkDto.AddingDate,
                    UnreliableDate = unreliableSkDto.UnreliableDate,
                    RelevanceDate = unreliableSkDto.RelevanceDate,
                    Biin = unreliableSkDto.Bin
                };
            foreach (var unreliableSkDto in unreliableSkDtos)
            {
                await _webUnreliableSkContext.UnreliableSks.Upsert(unreliableSkDto)
                    .On(x => x.Biin).RunAsync();
            }

            var minDate = await _parsedUnreliableSkContext.UnreliableSkDtos.MinAsync(x => x.RelevanceDate);
            _webUnreliableSkContext.UnreliableSks.RemoveRange(_webUnreliableSkContext.UnreliableSks.Where(x=>x.RelevanceDate<minDate));
            await _webUnreliableSkContext.SaveChangesAsync();
            await _parsedUnreliableSkContext.Database.ExecuteSqlRawAsync("truncate avroradata.unreliable_suppliers_sk restart identity;");
            await _webUnreliableSkContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");
        }
    }
}