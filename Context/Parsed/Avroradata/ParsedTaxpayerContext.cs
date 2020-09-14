using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTaxpayerContext : ParsedAvroradataContext
    {
        public DbSet<TaxpayerDto> TaxpayerDtos { get; set; }
    }
}