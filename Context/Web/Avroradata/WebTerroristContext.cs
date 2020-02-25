using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTerroristContext:DbContext
    {
        public WebTerroristContext(DbContextOptions<WebTerroristContext> options):base(options){}
        public WebTerroristContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql("");
        }
        public DbSet<Terrorist> Terrorists { get; set; }
    }
}