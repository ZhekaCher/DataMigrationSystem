using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTaxDetailsContext:ParsedContext
    {
        public DbSet<TaxDetailsDto> TaxDetailsDtos { get; set; }
    }
}