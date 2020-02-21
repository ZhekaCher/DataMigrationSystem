using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{

    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:23:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'INPUT'
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
                      "Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'evgeniy'; Integrated Security=true; Pooling=true;");
           }
    }
}