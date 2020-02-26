using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebProducerSkContext: WebContext
    {
        public DbSet<ProducersSk> ProducerSks { get; set; }
    }
}