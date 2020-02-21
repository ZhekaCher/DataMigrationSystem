using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebLeavingRestrictionContext: DbContext

    {
        public WebLeavingRestrictionContext(DbContextOptions<WebLeavingRestrictionContext> options)
            : base(options)
        {
            
        }

        public WebLeavingRestrictionContext()
        {
           
        }

        public DbSet<IndividualLeavingRestriction> IndividualLeavingRestrictions { get; set; }    
        public DbSet<CompanyLeavingRestriction> CompanyLeavingRestrictions { get; set; }    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}