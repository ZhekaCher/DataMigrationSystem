using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;


namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedGoszakupContext : ParsedContext
    {
        public DbSet<AnnouncementGoszakupDto> Announcements { get; set; }
        public DbSet<LotGoszakupDto> Lots { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                // .HasForeignKey(x => x.TenderId)
                // .HasPrincipalKey(x => x.FullId);
                ;
        }
    }
}