using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTerroristContext:DbContext
    {
        public ParsedTerroristContext(DbContextOptions<ParsedTerroristContext> options):base(options){}
        public ParsedTerroristContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql("");
        }
        public DbSet<TerroristDto> TerroristDtos { get; set; }
    }
}