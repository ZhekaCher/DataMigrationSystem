using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Parsed.Monitoring;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{

    /// @author Yevgeniy Cherdantsev
    /// @date 26.02.2020 17:50:16
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'parser_monitoring'
    /// </summary>

    public class ParserMonitoringContext : ParsedAvroradataContext
    {
        public DbSet<ParserMonitoring> ParserMonitorings { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("monitoring");
        }
    }
}