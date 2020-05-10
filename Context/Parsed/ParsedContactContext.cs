using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedContactContext : ParsedContext
    {
        public DbSet<ContactDto> ContactsDtos { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactDto>().HasKey(x => new {x.Bin, x.Source});
          
        }
        
    }
}