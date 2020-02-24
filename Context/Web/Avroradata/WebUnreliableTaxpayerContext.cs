using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnreliableTaxpayerContext: DbContext
    {
        public WebUnreliableTaxpayerContext(DbContextOptions<WebUnreliableTaxpayerContext> options) : base(options)
        {
            
        }

        public WebUnreliableTaxpayerContext()
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
        
        public DbSet<UnreliableTaxpayer> UnreliableTaxpayers { get; set; }    
    }
}