using Microsoft.EntityFrameworkCore;
using SomeNamespace;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedAnnouncementNadlocContext : DbContext
    {
        public DbSet<AnnouncementNadlocDto> AnnouncementNadlocDtos { get; set; }

        public ParsedAnnouncementNadlocContext(DbContextOptions<ParsedAnnouncementNadlocContext> options) :
            base(options)
        {
        }

        public ParsedAnnouncementNadlocContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.2.24'; Database = 'intender_production'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'nadloc'; Integrated Security=true; Pooling=true;");
        }
    }
}