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
            var reliableSkDtos = _parsedReliableSkContext.ReliableSkDtos
                .Select(x => new ReliableSk
                {
                    Reason = x.Reason,
                    AddingDate = x.AddingDate,
                    RelevanceDate = x.RelevanceDate,
                    Biin = x.Bin
                });
            foreach (var reliableSkDto in reliableSkDtos)
            {
                await _webReliableSkContext.ReliableSks.Upsert(reliableSkDto).On(x => x.Biin).RunAsync();
            }
        }
    }
}