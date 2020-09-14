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
                    Ceo = producersSkDto.Ceo,
                    Contacts = producersSkDto.Contacts,
                    Bin = producersSkDto.Bin,
                    ProducerType = 1,
                    RelevanceDate = producersSkDto.RelevanceDate
                };
            foreach (var producersSkDto in producersSkDtos)
            {
                await _webProducerSkContext.ProducerSks.Upsert(producersSkDto).On(x => x.Bin).RunAsync();
            }

            var producerProductsDtos = from producerProductsDto in _parsedProducerSkContext.ProducerProductsDtos
                select new ProducerProducts
                {
                    Bin = producerProductsDto.Bin,
                    Products = producerProductsDto.Products,
                    AddingDate = producerProductsDto.AddingDate,
                    Certificates = producerProductsDto.Certificates,
                    Positions = producerProductsDto.Positions,
                    DocDate = producerProductsDto.DocumentDate,
                    Validity = producerProductsDto.Validity,
                    RelevanceDate = producerProductsDto.RelevanceDate
                };

            await _webProducerSkContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.samruk_producer_company_products restart identity;");
            
            foreach (var producerProductsDto in producerProductsDtos)
            {
                await _webProducerSkContext.ProducerProductses.AddAsync(producerProductsDto);
            }
            await _webProducerSkContext.SaveChangesAsync();
            
            var lastDate = _webProducerSkContext.ProducerSks.Max(x => x.RelevanceDate).Date;
            _webProducerSkContext.ProducerSks.RemoveRange(_webProducerSkContext.ProducerSks.Where(x=>x.RelevanceDate<lastDate));
            await _webProducerSkContext.SaveChangesAsync();
            
            await _parsedProducerSkContext.Database.ExecuteSqlRawAsync("truncate avroradata.producers_sk, avroradata.producer_products_sk restart identity;");
        }
    }
}