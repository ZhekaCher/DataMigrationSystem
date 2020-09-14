using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTerroristContext: WebAvroradataContext
    {
        public DbSet<Terrorist> Terrorists { get; set; }
    }
}