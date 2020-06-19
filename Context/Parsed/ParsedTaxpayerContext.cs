using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTaxpayerContext : ParsedContext
    {
        public DbSet<TaxpayerDto> TaxpayerDtos { get; set; }
    }
}