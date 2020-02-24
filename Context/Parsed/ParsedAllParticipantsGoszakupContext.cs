using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 13:32:44
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'all_participants_goszakup'
    /// </summary>
    public class ParsedAllParticipantsGoszakupContext : DbContext
    {
        public DbSet<AllParticipantsGoszakupDto> AllParticipantsGoszakupDtos { get; set; }
        public ParsedAllParticipantsGoszakupContext(DbContextOptions<ParsedAllParticipantsGoszakupContext> options)
            : base(options)
        {
        }

        public ParsedAllParticipantsGoszakupContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 'localhost'; Database = 'new_database'; Port='5432'; User ID = 'postgres'; Password = 'toor'; Search Path = 'new_goszakup'; Integrated Security=true; Pooling=true;");
        }
    }
}