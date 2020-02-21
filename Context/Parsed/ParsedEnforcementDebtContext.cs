using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedEnforcementDebtContext: DbContext
    {
        public ParsedEnforcementDebtContext(DbContextOptions<ParsedEnforcementDebtContext> options)
            : base(options)
        {
            
        }

        public ParsedEnforcementDebtContext()
        {
        }

        public DbSet<EnforcementDebtDto> EnforcementDebtDtos { get; set; }    
        public DbSet<EnforcementDebtDetailDto> EnforcementDebtDetailDtos { get; set; }
        public DbSet<ParsedCompany> ParsedCompanies { get; set; }
        public DbSet<ParsedIndividual> ParsedIndividuals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnforcementDebtDto>().HasOne(x => x.DetailDto).WithOne(x => x.EnforcementDebtDto)
                .HasForeignKey<EnforcementDebtDetailDto>(x => x.Uid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
    }
}