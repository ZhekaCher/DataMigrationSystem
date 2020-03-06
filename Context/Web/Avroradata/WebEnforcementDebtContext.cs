using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEnforcementDebtContext: WebContext
    {

        public DbSet<EnforcementDebt> EnforcementDebts { get; set; }    
        public DbSet<EnforcementDebtType> EnforcementDebtTypes { get; set; }
        public DbSet<EnforcementDebtHistory> EnforcementDebtHistories { get; set; }
    }
}