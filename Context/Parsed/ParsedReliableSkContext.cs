using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedReliableSkContext:DbContext
    {
        public ParsedReliableSkContext(DbContextOptions<ParsedReliableSkContext> options)
            :base(options){}
        public ParsedReliableSkContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        
        public DbSet<ReliableSkDto> ReliableSkDtos { get; set; }
    }
}