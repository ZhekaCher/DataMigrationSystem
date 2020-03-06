using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using NLog;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class SamrukParticipantsMigrationService:MigrationService
    {


        private readonly  object _forLock  = new object();
        private int _counter;
        public SamrukParticipantsMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int threadNum)
        {
            {
                var webSamrukParticipantsContext = new WebSamrukParticipantsContext();
                var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
                var samrukParticipantsDtos =
                    from samrukParticipantsDto in parsedSamrukParticipantsContext.SamrukParticipantsDtos
                    where samrukParticipantsDto.Id % NumOfThreads == threadNum
                    select new SamrukParticipants
                    {
                        CodeBin = samrukParticipantsDto.CodeBin,
                        DirectorFullname = samrukParticipantsDto.DirectorFullname,
                        DirectorIin = samrukParticipantsDto.DirectorIin,
                        Customer = samrukParticipantsDto.Customer,
                        Supplier = samrukParticipantsDto.Supplier,
                        RelevanceDate = samrukParticipantsDto.RelevanceDate
                    };
                foreach (var samrukParticipantsDto in samrukParticipantsDtos)
                {
                    await webSamrukParticipantsContext.SamrukParticipantses.Upsert(samrukParticipantsDto)
                        .On(x => x.CodeBin).RunAsync();
                    lock (_forLock)
                    {
                        Logger.Trace(_counter++);
                    }
                }

            }
        }

        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }
            await Task.WhenAll(tasks);
            var  webSamrukParticipantsContext = new WebSamrukParticipantsContext();
            var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
            var minDate = parsedSamrukParticipantsContext.SamrukParticipantsDtos.Min(x => x.RelevanceDate);
            webSamrukParticipantsContext.SamrukParticipantses.RemoveRange(webSamrukParticipantsContext.SamrukParticipantses.Where(x=>x.RelevanceDate<minDate));
            await webSamrukParticipantsContext.SaveChangesAsync();
            await parsedSamrukParticipantsContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.samruk_all_participants restart identity");
        }
    }
}