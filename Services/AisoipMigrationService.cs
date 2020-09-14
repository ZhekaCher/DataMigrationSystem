using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class AisoipMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter = 0;
        private readonly Dictionary<string, long> _dictionary = new Dictionary<string, long>();
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
            await Migrate();
           
            await using var parsedAisoipContext = new ParsedAisoipContext();
            await parsedAisoipContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.aisoip_companies, avroradata.aisoip_details restart identity;");

        }

        private async Task Insert(AisoipDto aisoip)
        {
            await using var webAisoipContext = new WebAisoipContext();
            webAisoipContext.ChangeTracker.AutoDetectChangesEnabled = false;
            await webAisoipContext.Aisoip.Upsert(new Aisoip
            {
                Result = aisoip.Result,
                Biin = aisoip.Biin,
                RelevanceDate = aisoip.RelevanceDate,
                AresId = _dictionary[aisoip.Title]
            }).On(x => new {x.Biin, x.AresId}).RunAsync();
            webAisoipContext.AisoipDetails.RemoveRange(webAisoipContext.AisoipDetails.Where(x => x.Biin == aisoip.Biin && x.AresId == _dictionary[aisoip.Title]));
            await webAisoipContext.SaveChangesAsync();
            if (aisoip.Result)
            {
                await webAisoipContext.AisoipDetails.AddRangeAsync(aisoip.AisoipDetailsDtos.Select(x=> new AisoipDetails
                {
                    Biin = aisoip.Biin,
                    AresId =_dictionary[aisoip.Title], 
                    Date = x.Date,
                    Address = x.Address,
                    Name = x.Name,
                    Tel = x.Tel,
                    RelevanceDate = x.RelevanceDate
                }));
                await webAisoipContext.SaveChangesAsync();
            }
            lock (_forLock)
            {
                Logger.Trace(_counter++);
            }
        }
        private async Task Migrate()
        {
            var tasks = new List<Task>();
            await using var parsedAisoipContext = new ParsedAisoipContext();
            var aisoips = parsedAisoipContext
                .AisoipDtos
                .AsNoTracking()
                .Include(x => x.AisoipDetailsDtos); 
            foreach (var aisoip in aisoips)
            {
                tasks.Add(Insert(aisoip));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            await Task.WhenAll(tasks);

        }

        private async Task MigrateReferences()
        {
            await using var parsedAisoipContext = new ParsedAisoipContext();
            await using var webAisoipContext = new WebAisoipContext();
            var aisoipLists = parsedAisoipContext.AisoipDtos.Select(x=>new AisoipList
            {
                Name = x.Title
            }).Distinct();
            await webAisoipContext.AisoipLists.UpsertRange(aisoipLists).On(x => x.Name).RunAsync();
            foreach (var aisoipList in webAisoipContext.AisoipLists)
            {
                _dictionary[aisoipList.Name] = aisoipList.Id;
            }
        }
    }
}