using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebBusinessReesterContext : WebAvroradataContext
    {
        public DbSet<BusinessReester> BusinessReesters { get; set; }
        public DbSet<BusinessReesterOwnershipType> BusinessReesterOwnershipTypes { get; set; }
    }
}