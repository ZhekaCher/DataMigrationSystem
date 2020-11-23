using System.Collections.Generic;
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
    public class GosReesterMigrationService : MigrationService
    {
        private readonly WebGosRessterContext _webGosRessterContext;
        private readonly ParsedGosReesterContext _parsedGosReesterContext;
        private readonly object _forLock;
        private int _counter;
        
        public GosReesterMigrationService(int numOfThreads = 5)
        {
            _webGosRessterContext = new WebGosRessterContext();
            _parsedGosReesterContext = new ParsedGosReesterContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for(var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            await Task.WhenAll(tasks);
            
            var gosReesterContext = new ParsedGosReesterContext();
            await gosReesterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.gos_reester restart identity;");

        }

        private async Task Migrate(int threads)
        {
            var gosReesterDtos = _parsedGosReesterContext.GosReesterDtos;
            foreach (var gosReesterDto in gosReesterDtos.Where(x=>x.Id%NumOfThreads==threads))
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