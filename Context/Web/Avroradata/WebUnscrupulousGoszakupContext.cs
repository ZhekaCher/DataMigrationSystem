using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:02:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'unscrupulous_goszakup'
    /// </summary>

    public class WebUnscrupulousGoszakupContext : DbContext
    {
        
        
        public DbSet<UnscrupulousGoszakup> UnscrupulousGoszakup { get; set; }
        public WebUnscrupulousGoszakupContext(DbContextOptions<WebUnscrupulousGoszakupContext> options)
            : base(options)
        {

        }

        public WebUnscrupulousGoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}