using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEnforcementDebtContext: DbContext
    {
        public WebEnforcementDebtContext(DbContextOptions<WebEnforcementDebtContext> options)
            : base(options)
        {
        }

        public WebEnforcementDebtContext()
        {
            
        }

        public DbSet<CompanyEnforcementDebt> CompanyEnforcementDebts { get; set; }    
        public DbSet<EnforcementDebtType> EnforcementDebtTypes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}