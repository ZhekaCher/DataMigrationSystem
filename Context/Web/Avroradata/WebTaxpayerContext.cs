using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTaxpayerContext : WebContext
    {
        public DbSet<Taxpayer> Taxpayers { get; set; }
        public DbSet<TypeOfService> TypeOfServices { get; set; }
    }
}