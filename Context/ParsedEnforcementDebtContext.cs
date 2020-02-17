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
        public DbSet<EnforcementDebtDto> EnforcementDebtDtos { get; set; }    
        public DbSet<EnforcementDebtDetailDto> EnforcementDebtDetailDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}