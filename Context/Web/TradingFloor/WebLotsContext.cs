using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.TradingFloor
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:20:01
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'lots'
    /// </summary>


    public class WebLotsContext : DbContext
    {
        public DbSet<Lots> Lots { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        public WebLotsContext(DbContextOptions<WebLotsContext> options)
            : base(options)
        {
            
        }

        public WebLotsContext()
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.1.158'; Database = 'avroradata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'trading_floor'; Integrated Security=true; Pooling=true;");
        }
    }
}