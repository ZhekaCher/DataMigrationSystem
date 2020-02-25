using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTop100GoszakupContext: DbContext
    {
        
        public DbSet<Top100SuppliersGoszakup> Top100Suppliersgoszakup { get; set; }    
        public DbSet<Top100CustomersGoszakup> Top100CustomersGoszakup { get; set; }    
        public WebTop100GoszakupContext(DbContextOptions<WebTop100GoszakupContext> options)
            : base(options)
        {

        }

        public WebTop100GoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
    }
}