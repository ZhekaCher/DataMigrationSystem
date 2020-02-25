using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnreliableSkContext:DbContext
    {
        public WebUnreliableSkContext(DbContextOptions<WebUnreliableSkContext> options)
            : base(options) { }
        public WebUnreliableSkContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        public DbSet<UnreliableSk> UnreliableSks { get; set; }
    }
}