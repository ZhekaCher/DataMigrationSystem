using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedNdsDetailsContext:ParsedAvroradataContext
    {
        public DbSet<NdsDetailsDto> NdsDetailsDtos { get; set; }
    }
}