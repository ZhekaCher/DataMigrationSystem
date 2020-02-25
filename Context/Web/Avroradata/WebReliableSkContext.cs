using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebReliableSkContext:DbContext
    {
        
        public WebReliableSkContext(DbContextOptions<WebReliableSkContext> options)
            :base(options){}
        public WebReliableSkContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        
        public DbSet<ReliableSk> ReliableSks { get; set; }
    }
}