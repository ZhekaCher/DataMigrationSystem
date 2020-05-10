using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebContactContext : WebContext
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactCopy> ContactCopies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().HasKey(x => new {x.Bin, x.Source});
            modelBuilder.Entity<ContactCopy>().HasKey(x => new {x.Bin, x.Source});
        }
        
    }
}