using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;
using AisoipList = DataMigrationSystem.Models.Parsed.AisoipList;

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
            var aisoips = from aisoip in ParsedAisoipContext.AisoipDtos 
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

            foreach (var aisoip in aisoips)
            {
                await webAisoipContext.Aisoip.Upsert(aisoip).On(x => x.Biin).RunAsync();
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

            var aisoipLists = parsedAisoipContext.AisoipLists.Select(x =>new {x.Id,x.Name});
            foreach (var aisoipList in aisoipLists)
            {
                var aisoipL = new AisoipList
                {
                    Id = aisoipList.Id,
                    Name = aisoipList.Name
                };
                await webAisoipContext.AisoipLists.Upsert(aisoipL).On(x => x.Name).RunAsync();
            }
        }
    }
}