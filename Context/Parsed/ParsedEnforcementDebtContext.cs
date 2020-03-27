using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedEnforcementDebtContext : ParsedContext
    {
        public DbSet<EnforcementDebtDto> EgovEnforcementDebtDtos { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyBinDto>()
                .HasMany(x => x.EnforcementDebtDtos)
                .WithOne()
                .HasPrincipalKey(x => x.Code)
                .HasForeignKey(x => x.IinBin);
        }
    }
}