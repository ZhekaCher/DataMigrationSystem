using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:23:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'announcements_goszakup'
    /// </summary>
    public class ParsedAnnouncementGoszakupContext : DbContext
    {
        public DbSet<AnnouncementGoszakupDto> AnnouncementGoszakupDtos { get; set; }

        public ParsedAnnouncementGoszakupContext(DbContextOptions<ParsedAnnouncementGoszakupContext> options)
            : base(options)
        {
        }

        public ParsedAnnouncementGoszakupContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 'localhost'; Database = 'new_database'; Port='5432'; User ID = 'postgres'; Password = 'toor'; Search Path = 'new_goszakup'; Integrated Security=true; Pooling=true;");
        }
    }
}