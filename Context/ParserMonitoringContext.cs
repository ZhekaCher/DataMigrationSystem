using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{

    /// @author Yevgeniy Cherdantsev
    /// @date 26.02.2020 17:50:16
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'parser_monitoring'
    /// </summary>

    public class ParserMonitoringContext : ParsedContext
    {
        public DbSet<ParserMonitoring> ParserMonitorings { get; set; }
    }
}