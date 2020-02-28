using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTerroristContext : ParsedContext
    {
        public DbSet<IndividualIin> IndividualIins { get; set; }
        public DbSet<TerroristDto> TerroristDtos { get; set; }
    }
}