using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:02:33
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'unscrupulouses_goszakup'
    /// </summary>
    public class ParsedUnscrupulousGoszakupContext : ParsedContext
    {
        public DbSet<UnscrupulousGoszakupDto> UnscrupulousGoszakupDtos { get; set; }
    }
}