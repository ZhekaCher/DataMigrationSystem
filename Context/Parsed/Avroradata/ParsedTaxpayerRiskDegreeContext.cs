using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTaxpayerRiskDegreeContext:ParsedAvroradataContext
    {
        public DbSet<TaxpayerRiskDegreeDto> TaxpayerRiskDegreeDtos { get; set; }
    }
}