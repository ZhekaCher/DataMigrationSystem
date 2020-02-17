using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class WebEnforcementDebtContext: DbContext
    {
        public WebEnforcementDebtContext(DbContextOptions<WebEnforcementDebtContext> options)
            : base(options)
        {
        }
        public DbSet<EnforcementDebt> EnforcementDebts { get; set; }    
        public DbSet<EnforcementDebtType> EnforcementDebtTypes { get; set; }
    }
}