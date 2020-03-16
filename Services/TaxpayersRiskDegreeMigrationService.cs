using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TaxpayersRiskDegreeMigrationService:MigrationService
    {

        private readonly WebTaxpayerRiskDegreeContext _webTaxpayerRiskDegreeContext;
        private readonly ParsedTaxpayerRiskDegreeContext _parsedTaxpayerRiskDegreeContext;

        public TaxpayersRiskDegreeMigrationService(int numOfThreats = 1)
        {
            NumOfThreads = numOfThreats;
            _webTaxpayerRiskDegreeContext = new WebTaxpayerRiskDegreeContext();
            _parsedTaxpayerRiskDegreeContext = new ParsedTaxpayerRiskDegreeContext();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var taxpayerRiskDegrees =
                from taxpayerRiskDegreeDto in _parsedTaxpayerRiskDegreeContext.TaxpayerRiskDegreeDtos
                select new TaxpayerRiskDegree
                {
                    Id = taxpayerRiskDegreeDto.Id,
                    Bin = taxpayerRiskDegreeDto.Bin,
                    Degree = taxpayerRiskDegreeDto.Degree,
                    CalculationRelevanceDate = taxpayerRiskDegreeDto.CalculationRelevanceDate,
                    RelevanceDateFrom = taxpayerRiskDegreeDto.RelevanceDateFrom,
                    RelevanceDateTo = taxpayerRiskDegreeDto.RelevanceDateTo
                };
            foreach (var taxpayerRiskDegree in taxpayerRiskDegrees)
            {
                await _webTaxpayerRiskDegreeContext.TaxpayerRiskDegrees.Upsert(taxpayerRiskDegree).On(x=>x.Bin).RunAsync();
            }
        }
    }
}