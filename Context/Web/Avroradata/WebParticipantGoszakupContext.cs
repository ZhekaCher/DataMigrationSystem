using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 13:43:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'participant_goszakup'
    /// </summary>

    public class WebParticipantGoszakupContext : WebContext
    {
        public DbSet<ParticipantGoszakup> ParticipantsGoszakup { get; set; }
        
    }
}