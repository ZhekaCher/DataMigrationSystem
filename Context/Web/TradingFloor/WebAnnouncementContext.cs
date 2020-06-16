using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.TradingFloor
{

    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:21:33
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'announcement'
    /// </summary>

    public class WebAnnouncementContext : WebContext
    {
    
        public DbSet<Announcement> Announcements { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trading_floor");
        }
    }
}