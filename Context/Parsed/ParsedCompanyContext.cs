using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCompanyContext: DbContext
    {
        public ParsedCompanyContext(DbContextOptions<ParsedCompanyContext> options)
            : base(options)
        {
            
        }

        public ParsedCompanyContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<CompanyDto> CompanyDtos { get; set; }    
    }
}