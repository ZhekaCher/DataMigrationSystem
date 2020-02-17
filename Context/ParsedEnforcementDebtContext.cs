using System.Security.Cryptography;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
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