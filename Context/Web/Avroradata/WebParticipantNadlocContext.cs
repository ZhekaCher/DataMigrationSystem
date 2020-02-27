using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebParticipantNadlocContext : DbContext
    {
        public  DbSet<ParticipantNadloc> ParticipantsNadloc { get; set; }

        public WebParticipantNadlocContext(DbContextOptions<WebParticipantNadlocContext> options) 
            : base(options)
        {
            
        }

        public WebParticipantNadlocContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.1.158'; Database = 'avroradata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'nadloc'; Integrated Security=true; Pooling=true;");

        }
    }
}