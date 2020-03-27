using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEnforcementDebtContext: WebContext
    {
        public DbSet<EgovEnforcementDebt> EgovEnforcementDebts { get; set; }    
        public DbSet<EnforcementDebtType> EnforcementDebtTypes { get; set; }
    }

}