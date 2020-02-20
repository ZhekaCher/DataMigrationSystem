using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class WebWantedIndividualContext : DbContext
    {
        public  WebWantedIndividualContext(DbContextOptions<WebWantedIndividualContext> options)
            :base(options)
        {
        }
        public WebWantedIndividualContext()
        {}
        public DbSet<WantedIndividual> WantedIndividuals { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<RaceType> RaceTypes { get; set; }
        public DbSet<Issued> Issueds { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ListType> ListTypes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}