using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class UnreliableSkMigrationService:MigrationService
    {
        private WebUnreliableSkContext _webUnreliableSkContext;
        private ParsedUnreliableSkContext _parsedUnreliableSkContext;

        public UnreliableSkMigrationService(int numOfThreads=1)
        {
            _webUnreliableSkContext = new WebUnreliableSkContext();
            _parsedUnreliableSkContext = new ParsedUnreliableSkContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
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

            var lastDate = _webUnreliableSkContext.UnreliableSks.Max(x => x.RelevanceDate).Date;
            _webUnreliableSkContext.UnreliableSks.RemoveRange(_webUnreliableSkContext.UnreliableSks.Where(x=>x.RelevanceDate<lastDate));
            await _webUnreliableSkContext.SaveChangesAsync();
        }
    }
}