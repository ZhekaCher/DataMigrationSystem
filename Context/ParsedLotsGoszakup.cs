using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:49:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'lots_goszakup'
    /// </summary>

    public class ParsedLotsGoszakup : DbContext
    {
        public ParsedLotsGoszakup(DbContextOptions<ParsedLotsGoszakup> options)
            : base(options)
        {

        }

        public ParsedLotsGoszakup()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = ''; Database = ''; Port=''; User ID = ''; Password = ''; Search Path = ''; Integrated Security=true; Pooling=true;");
        }
    }
}