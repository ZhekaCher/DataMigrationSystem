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
    public class AisoipMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter = 0;
        
        public AisoipMigrationService(int numOfThreads = 30)
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
            await MigrateReferences();
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
            await using var parsedAisoipContext = new ParsedAisoipContext();
            await using var webAisoipContext = new WebAisoipContext();
        }
        
        public async Task Migrate(int numThread)
        {
            await using var parsedAisoipContext = new ParsedAisoipContext();
            await using var webAisoipContext = new WebAisoipContext();
            var aisoips = from aisoip in parsedAisoipContext.AisoipDtos
                orderby aisoip.Id
                where aisoip.Id % NumOfThreads == numThread
                select new Aisoip
                {
                    Id = aisoip.Id,
                    Biin = aisoip.Biin,
                    AresId = aisoip.AresId,
                    Result = aisoip.Result,
                    RelevanceDate = aisoip.RelevanceDate
                };
            await using var parsedAisoipContext2 = new ParsedAisoipContext();
            foreach (var aisoip in aisoips)
            {
                var aisoipPoint = await parsedAisoipContext2.AisoipLists.FirstOrDefaultAsync(x => x.Id == aisoip.AresId);
                var newPoint = webAisoipContext.AisoipLists.FirstOrDefault(x => x.Name ==aisoipPoint.Name);
                aisoip.AresId = newPoint.Id;
                await webAisoipContext.Aisoip.Upsert(aisoip).On(x => new {x.Biin, x.AresId}).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }

        private async Task MigrateReferences()
        {
            await using var parsedAisoipContext = new ParsedAisoipContext();
            await using var webAisoipContext = new WebAisoipContext();
            var aisoipLists = parsedAisoipContext.AisoipLists.ToList();
            await webAisoipContext.AisoipLists.UpsertRange(aisoipLists).On(x => x.Name).RunAsync();
        }
    }
}