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

        public UnreliableSkMigrationService(int numOfThreads)
        {
            NumOfThreads = numOfThreads;
            _webUnreliableSkContext = new WebUnreliableSkContext();
            _parsedUnreliableSkContext = new ParsedUnreliableSkContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var unreliableSkDtos = _parsedUnreliableSkContext.UnreliableSkDtos
                .Select(x => new UnreliableSk
                {
                    Reason = x.Reason,
                    AddingDate = x.AddingDate,
                    UnreliableDate = x.UnreliableDate,
                    RelevanceDate = x.RelevanceDate,
                    Biin = x.Bin
                });
            foreach (var unreliableSkDto in unreliableSkDtos)
            {
                await _webUnreliableSkContext.UnreliableSks.Upsert(unreliableSkDto)
                    .On(x => x.Biin).RunAsync();
            }
        }
    }
}