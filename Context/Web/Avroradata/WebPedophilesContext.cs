using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebPedophilesContext : WebContext
    {
        public DbSet<Pedophile> Pedophiles { get; set; }
    }
}