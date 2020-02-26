using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTop100GoszakupContext: WebContext
    {
        public DbSet<Top100SuppliersGoszakup> Top100Suppliersgoszakup { get; set; }    
        public DbSet<Top100CustomersGoszakup> Top100CustomersGoszakup { get; set; }
    }
}