using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEnforcementDebtContext: WebContext
    {

        public DbSet<CompanyEnforcementDebt> CompanyEnforcementDebts { get; set; }    
        public DbSet<EnforcementDebtType> EnforcementDebtTypes { get; set; }
    }
}