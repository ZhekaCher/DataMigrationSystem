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
    public class NdsDetailsMigrationService:MigrationService
    {
        
        private readonly object _forLock;
        private int _total;
        private List<NdsDetailReason> _ndsDetailReasons;

        public NdsDetailsMigrationService(int numOfThreads = 20)
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

        private async Task MigrateAsync(int threadNum)
        {
            var webNdsDetailsContext = new WebNdsDetailsContext();
            var parsedNdsDetailsContext = new ParsedNdsDetailsContext();
            var ndsDetailsDtos = parsedNdsDetailsContext.NdsDetailsDtos.Where(x=>x.Id%NumOfThreads == threadNum);
            foreach (var ndsDetailsDto in ndsDetailsDtos)
            {
                var ndsDetails = DtoToEntity(ndsDetailsDto);
                await webNdsDetailsContext.NdsDetailses.Upsert(ndsDetails).On(x => x.Bin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_total--);
                }
            }
        }
        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }
            await Task.WhenAll(tasks);
        }

        private NdsDetails DtoToEntity(NdsDetailsDto ndsDetailsDto)
        {
            var ndsDetails = new NdsDetails
            {
                Rnn = ndsDetailsDto.Rnn,
                Bin = ndsDetailsDto.Bin,
                StartDate = ndsDetailsDto.StartDate,
                FinishDate = ndsDetailsDto.FinishDate,
                RelevanceDate = ndsDetailsDto.RelevanceDate,
                ReasonId = _ndsDetailReasons.FirstOrDefault(x => x.Name == ndsDetailsDto.Reason)?.Id
            };
            return ndsDetails;
        }
        private async Task MigrateReferences()
        {
            var webNdsDetailsContext = new WebNdsDetailsContext();
            var parsedNdsDetailsContext = new ParsedNdsDetailsContext();
            var reasons = parsedNdsDetailsContext.NdsDetailsDtos
                .Select(x => new {x.Reason}).Distinct();

            foreach (var distinct in reasons)
            {
                await webNdsDetailsContext.NdsDetailReasons.Upsert(new NdsDetailReason
                {
                    Name = distinct.Reason
                }).On(x => x.Name).RunAsync();
            }

            _ndsDetailReasons = webNdsDetailsContext.NdsDetailReasons.ToList();
        }
    }
}