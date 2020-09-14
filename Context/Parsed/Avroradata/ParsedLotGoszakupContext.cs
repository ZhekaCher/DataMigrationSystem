using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:49:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'lot_goszakup'
    /// </summary>
    public class ParsedLotGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<LotGoszakupDto> LotGoszakupDtos { get; set; }
    }
}