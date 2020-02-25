using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 13:32:44
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'participant_goszakup'
    /// </summary>
    public class ParsedParticipantGoszakupContext : DbContext
    {
        public DbSet<ParticipantGoszakupDto> ParticipantGoszakupDtos { get; set; }
        public ParsedParticipantGoszakupContext(DbContextOptions<ParsedParticipantGoszakupContext> options)
            : base(options)
        {
        }

        public ParsedParticipantGoszakupContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'evgeniy'; Integrated Security=true; Pooling=true;");
        }
    }
}