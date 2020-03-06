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
        private readonly WebSamrukParticipantsContext _webSamrukParticipantsContext;
        private readonly ParsedSamrukParticipantsContext _parsedSamrukParticipantsContext;

        public SamrukParticipantsMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            _webSamrukParticipantsContext = new WebSamrukParticipantsContext();
            _parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var samrukParticipantsDtos =
                from samrukParticipantsDto in _parsedSamrukParticipantsContext.SamrukParticipantsDtos
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
                await _webSamrukParticipantsContext.SamrukParticipantses.Upsert(samrukParticipantsDto)
                    .On(x => x.CodeBin).RunAsync();
            }

            var minDate = _parsedSamrukParticipantsContext.SamrukParticipantsDtos.Min(x => x.RelevanceDate);
            _webSamrukParticipantsContext.SamrukParticipantses.RemoveRange(_webSamrukParticipantsContext.SamrukParticipantses.Where(x=>x.RelevanceDate<minDate));
            await _webSamrukParticipantsContext.SaveChangesAsync();
            await _parsedSamrukParticipantsContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.samruk_all_participants restart identity");
            
            
        }
    }
}