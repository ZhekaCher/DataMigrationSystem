using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedEnforcementDebtContext : ParsedContext
    {
        public DbSet<EnforcementDebtDto> EnforcementDebtDtos { get; set; }    
        public DbSet<EnforcementDebtDetailDto> EnforcementDebtDetailDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnforcementDebtDto>().HasOne(x => x.DetailDto).WithOne(x => x.EnforcementDebtDto)
                .HasForeignKey<EnforcementDebtDetailDto>(x => x.Uid);
        }

    }
}