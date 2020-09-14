using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedGosReesterContext:ParsedAvroradataContext
    {
        public DbSet<GosReesterDto> GosReesterDtos { get; set; }
    }
}