using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedDisabilitiesOrgSkContext : DbContext
    {
        public ParsedDisabilitiesOrgSkContext(DbContextOptions<ParsedDisabilitiesOrgSkContext> options):base(options)
        { }
        
        public ParsedDisabilitiesOrgSkContext(){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = localhost; Database = avroradata; Port=5432; User ID = postgres; Password = admin2000; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
        
        public DbSet<DisabilitiesOrgSkDto> DisabilitiesOrgSkDtos { get; set; }
    }
}