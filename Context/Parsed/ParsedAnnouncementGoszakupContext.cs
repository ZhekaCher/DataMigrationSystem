using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:23:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'announcement_goszakup'
    /// </summary>
    public class ParsedAnnouncementGoszakupContext : ParsedContext
    {
        public DbSet<AnnouncementGoszakupDto> AnnouncementGoszakupDtos { get; set; } 
    }
}