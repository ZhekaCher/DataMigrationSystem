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

    public class WebAnnouncementContext : DbContext
    {
    
        public DbSet<Announcement> Announcements { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        public WebAnnouncementContext(DbContextOptions<WebAnnouncementContext> options)
            : base(options)
        {

        }

        public WebAnnouncementContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.1.158'; Database = 'avroradata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'trading_floor'; Integrated Security=true; Pooling=true;");
        }
    }
}