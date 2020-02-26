using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web
{
    public class WebContext: DbContext
    {

        public WebContext(DbContextOptions<WebContext> options)
            : base(options)
        {
        }

        public WebContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}