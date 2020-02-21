using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedWantedIndividualContext : DbContext
    {
        public ParsedWantedIndividualContext(DbContextOptions<ParsedWantedIndividualContext> options)
            : base(options) { }
        public ParsedWantedIndividualContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<WantedIndividualDto> WantedIndividualDtos { get; set; }
    }
}