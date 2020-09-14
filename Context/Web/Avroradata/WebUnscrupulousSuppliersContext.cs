using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnscrupulousSuppliersContext : WebAvroradataContext
    {
        public DbSet<UnscrupulousSuppliers> UnscrupulousSupplierses { get; set; }
    }
}