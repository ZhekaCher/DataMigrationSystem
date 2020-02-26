using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:49:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'lot_goszakup'
    /// </summary>
    public class ParsedLotGoszakupContext : ParsedContext
    {
        public DbSet<LotGoszakupDto> LotGoszakupDtos { get; set; }
    }
}