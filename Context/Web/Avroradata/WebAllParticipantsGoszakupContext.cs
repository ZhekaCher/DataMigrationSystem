using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 13:43:36
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'all_participants_goszakup'
    /// </summary>

    public class WebAllParticipantsGoszakupContext : DbContext
    {
        public DbSet<AllParticipantsGoszakup> AllParticipantsGoszakup { get; set; }
        public WebAllParticipantsGoszakupContext(DbContextOptions<WebAllParticipantsGoszakupContext> options)
            : base(options)
        {

        }

        public WebAllParticipantsGoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}