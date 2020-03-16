using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class NdsDetailsMigrationService:MigrationService
    {
        
        private readonly object _forLock;
        private int _total;

        public NdsDetailsMigrationService(int numOfThreads=1)
        {
            NumOfThreads = numOfThreads;
            using var parsedNdsDetailsContext = new ParsedNdsDetailsContext();
            _total = parsedNdsDetailsContext.NdsDetailsDtos.Count();
            _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            WebNdsDetailsContext webNdsDetailsContext = new WebNdsDetailsContext();
            ParsedNdsDetailsContext parsedNdsDetailsContext = new ParsedNdsDetailsContext();
            await MigrateReferences();
            var ndsDetailsDtos = parsedNdsDetailsContext.NdsDetailsDtos;
            foreach (var ndsDetailsDto in ndsDetailsDtos)
            {
                var ndsDetail = await DtoToEntity(ndsDetailsDto,webNdsDetailsContext);
                await webNdsDetailsContext.NdsDetailses.Upsert(ndsDetail).On(x => x.Bin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_total--);
                }
            }
        }

        private async Task<NdsDetails> DtoToEntity(NdsDetailsDto ndsDetailsDto, WebNdsDetailsContext webNdsDetailsContext)
        {
            var ndsDetails = new NdsDetails
            {
                Rnn = ndsDetailsDto.Rnn,
                Bin = ndsDetailsDto.Bin,
                StartDate = ndsDetailsDto.StartDate,
                FinishDate = ndsDetailsDto.FinishDate
            };
            if (ndsDetailsDto.Reason!=null)
            {
                ndsDetails.ReasonId = (await webNdsDetailsContext.NdsDetailReasons.FirstOrDefaultAsync(x=>x.Name == ndsDetailsDto.Reason)).Id;
            }
            return ndsDetails;
        }
        private async Task MigrateReferences()
        {
         WebNdsDetailsContext webNdsDetailsContext = new WebNdsDetailsContext();
         ParsedNdsDetailsContext parsedNdsDetailsContext = new ParsedNdsDetailsContext();
            var reasons = parsedNdsDetailsContext.NdsDetailsDtos
                .Select(x => new {x.Reason}).Distinct();

            foreach (var distinct in reasons)
            {
                await webNdsDetailsContext.NdsDetailReasons.Upsert(new NdsDetailReason
                {
                    Name = distinct.Reason
                }).On(x=>x.Name).RunAsync();
            }
        }
    }
}