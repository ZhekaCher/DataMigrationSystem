using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class ParsedCourtCaseContext: DbContext
    {
        public ParsedCourtCaseContext(DbContextOptions<ParsedCourtCaseContext> options)
            : base(options)
        {
            
        }

        public ParsedCourtCaseContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<CourtCaseDto> CourtCaseDtos { get; set; }    
        public DbSet<CourtCaseEntityDto> CourtCaseEntityDtos { get; set; }    
        public DbSet<ParsedCompany> ParsedCompanies { get; set; }
        public DbSet<ParsedIndividual> ParsedIndividuals { get; set; }
    }
}