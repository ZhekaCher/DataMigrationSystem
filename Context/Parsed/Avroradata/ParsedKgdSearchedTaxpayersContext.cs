using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedKgdSearchedTaxpayersContext : ParsedAvroradataContext
    {
        public DbSet<KgdSearchedTaxpayersDto> KgdSearchedTaxpayersDto { get; set; }
    }
}