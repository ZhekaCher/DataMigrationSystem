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


    public class WebLotContext : DbContext
    {
        public DbSet<Lot> Lots { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        public WebLotContext(DbContextOptions<WebLotContext> options)
            : base(options)
        {
            
        }

        public WebLotContext()
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.1.158'; Database = 'avroradata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'trading_floor'; Integrated Security=true; Pooling=true;");
        }
    }
}