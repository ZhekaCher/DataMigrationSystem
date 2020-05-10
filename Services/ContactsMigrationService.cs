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
    public class ContactsMigrationService : MigrationService
    {
        private int _total;
     
        private readonly object _lock = new object();

        public ContactsMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedContactContext = new ParsedContactContext();
            _total = parsedContactContext.ContactsDtos.Count();
           
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webContactContext = new WebContactContext();
            await using var parsedContactContext = new ParsedContactContext();
            
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
            await parsedContactContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.contacts restart identity cascade;");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            await using var webContactContext = new WebContactContext();
            await using var parsedContactContext = new ParsedContactContext();

            foreach (var dto in parsedContactContext.ContactsDtos.Where(x=>x.Id % NumOfThreads == threadNum))
            {
                var contact = new Contact
                {
                    Bin = dto.Bin,
                    Email = dto.Email,
                    Website = dto.Website,
                    Telephone = dto.Telephone,
                    Source = dto.Source,
                    RelevanceDate = dto.RelevanceDate
                };
                
                await webContactContext.Contacts.Upsert(contact).On(x=>new {x.Bin,x.Source }).RunAsync();
                lock (_lock)
                {
                    Logger.Trace($"Left {--_total}");
                }


            }
        }
    }
}