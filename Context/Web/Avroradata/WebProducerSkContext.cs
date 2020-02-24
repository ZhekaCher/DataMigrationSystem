using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebProducerSkContext:DbContext
    {
        public WebProducerSkContext(DbContextOptions<WebProducerSkContext> options)
            :base(options){}
        public WebProducerSkContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        public DbSet<ProducersSk> ProducerSks { get; set; }
    }
}