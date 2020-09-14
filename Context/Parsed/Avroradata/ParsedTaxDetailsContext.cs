using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTaxDetailsContext:ParsedAvroradataContext
    {
        public DbSet<TaxDetailsDto> TaxDetailsDtos { get; set; }
    }
}