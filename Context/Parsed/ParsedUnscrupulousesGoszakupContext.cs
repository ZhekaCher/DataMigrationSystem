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

    public class ParsedUnscrupulousesGoszakupContext : DbContext
    {
        public DbSet<UnscrupulousesGoszakupDto> UnscrupulousesGoszakupDtos { get; set; }
        public ParsedUnscrupulousesGoszakupContext(DbContextOptions<ParsedUnscrupulousesGoszakupContext> options)
            : base(options)
        {

        }

        public ParsedUnscrupulousesGoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
          optionsBuilder.UseNpgsql(
                        "Server = 'localhost'; Database = 'new_database'; Port='5432'; User ID = 'postgres'; Password = 'toor'; Search Path = 'new_goszakup'; Integrated Security=true; Pooling=true;");
                }
    }
}