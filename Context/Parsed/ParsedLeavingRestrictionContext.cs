using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedLeavingRestrictionContext : DbContext
    {
        public ParsedLeavingRestrictionContext(DbContextOptions<ParsedLeavingRestrictionContext> options)
            : base(options)
        {
            
        }

        public ParsedLeavingRestrictionContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<LeavingRestrictionDto> LeavingRestrictionDtos { get; set; }    
        public DbSet<ParsedCompany> ParsedCompanies { get; set; }
        public DbSet<ParsedIndividual> ParsedIndividuals { get; set; }
    }
}