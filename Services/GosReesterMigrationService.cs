using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GosReesterMigrationService : MigrationService
    {
        private readonly WebGosRessterContext _webGosRessterContext;
        private readonly ParsedGosReesterContext _parsedGosReesterContext;
        private readonly object _forLock;
        private int _counter;
        
        public GosReesterMigrationService(int numOfThreads = 1)
        {
            _webGosRessterContext = new WebGosRessterContext();
            _parsedGosReesterContext = new ParsedGosReesterContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        public override async Task StartMigratingAsync()
        {
            await Migrate();
            var gosReesterContext = new ParsedGosReesterContext();
            await gosReesterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.gos_reester restart identity;");

        }

        private async Task Migrate()
        {
            var gosReesterDtos = _parsedGosReesterContext.GosReesterDtos;
            await foreach (var gosReesterDto in gosReesterDtos)
            {
                var gosRessters = new GosReester
                {
                    RegNum = gosReesterDto.RegNum,
                    RegDate = gosReesterDto.RegDate,
                    Status = gosReesterDto.Status,
                    Owner =gosReesterDto.Owner,
                    Name = gosReesterDto.Name,
                    MktuText = gosReesterDto.MktuText,
                    Path = gosReesterDto.Path,
                    ValidationDate = gosReesterDto.ValidationDate,
                    NewsletterNum = gosReesterDto.NewsletterNum,
                    NewsletterDate = gosReesterDto.NewsletterDate,
                    RelevanceDate = gosReesterDto.RelevanceDate,
                    Bin = gosReesterDto.Bin
                }; 
                await _webGosRessterContext.GosReesters.Upsert(gosRessters).On(x => x.RegNum).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}