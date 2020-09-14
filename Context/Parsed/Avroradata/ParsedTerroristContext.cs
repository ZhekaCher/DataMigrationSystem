using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTerroristContext : ParsedAvroradataContext
    {
        public DbSet<IndividualIin> IndividualIins { get; set; }
        public DbSet<TerroristDto> TerroristDtos { get; set; }
    }
}