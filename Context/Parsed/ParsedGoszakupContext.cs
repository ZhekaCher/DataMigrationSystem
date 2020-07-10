using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;


namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedGoszakupContext : ParsedContext
    {
        public DbSet<AnnouncementGoszakupDto> AnnouncementGoszakupDtos { get; set; }
        public DbSet<LotGoszakupDto> LotGoszakupDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.TrdBuyNumberAnno)
                .HasPrincipalKey(x => x.NumberAnno);
        }
    }
}