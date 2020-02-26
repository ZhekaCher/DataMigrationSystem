using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTerroristContext : ParsedContext
    {
        public DbSet<TerroristDto> TerroristDtos { get; set; }
    }
}