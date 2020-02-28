using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class ProducersSkMigrationService:MigrationService
    {
        private readonly WebProducerSkContext _webProducerSkContext;
        private readonly ParsedProducerSkContext _parsedProducerSkContext;

        public ProducersSkMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            _webProducerSkContext = new WebProducerSkContext();
            _parsedProducerSkContext = new ParsedProducerSkContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            /*var producersSkDtos = _parsedProducerSkContext.ProducerSkDtos
                .Select(x => new ProducersSk
                {
                    Id = x.Id,
                    Bin = x.Bin,
                    ProducerType = x.ProducerType,
                    RelevanceDate = x.RelevanceDate
                });*/
            var producersSkDtos = from producersSkDto in _parsedProducerSkContext.ProducerSkDtos
                join company in _parsedProducerSkContext.CompanyBinDtos
                    on producersSkDto.Bin equals company.Code
                    select new ProducersSk
                {
                    Id = producersSkDto.Id,
                    Bin = producersSkDto.Bin,
                    ProducerType = 1,
                    RelevanceDate = producersSkDto.RelevanceDate
                };
            foreach (var producersSkDto in producersSkDtos)
            {
                await _webProducerSkContext.ProducerSks.Upsert(producersSkDto).On(x => x.Bin).RunAsync();
            }

            var lastDate = _parsedProducerSkContext.ProducerSkDtos.Max(x => x.RelevanceDate).Date;
            _webProducerSkContext.ProducerSks.RemoveRange(_webProducerSkContext.ProducerSks.Where(x=>x.RelevanceDate<lastDate));
            await _webProducerSkContext.SaveChangesAsync();
        }
    }
}