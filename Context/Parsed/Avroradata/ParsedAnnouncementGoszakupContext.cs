using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:23:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'announcement_goszakup'
    /// </summary>
    public class ParsedAnnouncementGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<AnnouncementGoszakupDto> AnnouncementGoszakupDtos { get; set; } 
    }
}