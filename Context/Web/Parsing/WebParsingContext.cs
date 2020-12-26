using DataMigrationSystem.Models.Web.Parsing;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Parsing
{
    public class WebParsingContext : DbContext
    {
        public DbSet<RelevanceDate> RelevanceDates { get; set; }
        
        public WebParsingContext(DbContextOptions<WebParsingContext> options)
            : base(options)
        {
        }

        public WebParsingContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 185.35.223.92; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
    }
}