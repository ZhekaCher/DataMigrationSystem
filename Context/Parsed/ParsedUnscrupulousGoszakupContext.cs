using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:02:33
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'unscrupulouses_goszakup_context'
    /// </summary>
    public class ParsedUnscrupulousGoszakupContext : DbContext
    {
        public DbSet<UnscrupulousGoszakupDto> UnscrupulousGoszakupDtos { get; set; }

        public ParsedUnscrupulousGoszakupContext(DbContextOptions<ParsedUnscrupulousGoszakupContext> options)
            : base(options)
        {
        }

        public ParsedUnscrupulousGoszakupContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'evgeniy'; Integrated Security=true; Pooling=true;");
        }
    }
}