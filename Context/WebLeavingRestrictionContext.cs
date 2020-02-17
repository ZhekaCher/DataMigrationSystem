using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class WebLeavingRestrictionContext: DbContext

    {
        public WebLeavingRestrictionContext(DbContextOptions<WebLeavingRestrictionContext> options)
            : base(options)
        {
            
        }

        public DbSet<IndividualLeavingRestriction> IndividualLeavingRestrictions { get; set; }    
        public DbSet<CompanyLeavingRestriction> CompanyLeavingRestrictions { get; set; }    

    }
}