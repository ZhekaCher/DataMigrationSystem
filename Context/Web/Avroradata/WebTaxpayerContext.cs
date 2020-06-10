using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTaxpayerContext : WebContext
    {
        public DbSet<Taxpayer> Taxpayers { get; set; }
        public DbSet<TaxpayerType> TypeOfServices { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Taxpayer>().HasKey(x => new {x.Uin, x.TypeId});
        }
    }
}