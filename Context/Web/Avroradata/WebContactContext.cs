using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebContactContext : WebContext
    {
        public DbSet<ContactTelephone> ContactTelephones { get; set; }
        public DbSet<ContactWebsite> ContactWebsites { get; set; }
        public DbSet<ContactEmail> ContactEmails { get; set; }

    }
}