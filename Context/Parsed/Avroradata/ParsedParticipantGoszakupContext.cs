using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 13:32:44
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'participant_goszakup'
    /// </summary>
    public class ParsedParticipantGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<ParticipantGoszakupDto> ParticipantGoszakupDtos { get; set; }
    }
}