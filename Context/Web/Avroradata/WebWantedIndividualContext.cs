using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebWantedIndividualContext : WebContext
    {
        public DbSet<WantedIndividual> WantedIndividuals { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<RaceType> RaceTypes { get; set; }
        public DbSet<Issued> Issueds { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ListType> ListTypes { get; set; }
    }
}