using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedEnforcementDebtContext : ParsedAvroradataContext
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