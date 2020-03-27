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


    public class WebLotContext : WebContext
    {
        public DbSet<Lot> Lots { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trading_floor");
        }
    }
}