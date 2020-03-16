using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTaxpayerRiskDegreeContext:WebContext
    {
        public DbSet<TaxpayerRiskDegreeDto> TaxpayerRiskDegreeDtos { get; set; }
    }
}