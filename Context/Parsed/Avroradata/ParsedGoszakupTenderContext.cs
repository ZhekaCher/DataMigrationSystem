using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedGoszakupTenderContext : ParsedAvroradataContext
    {
        public DbSet<AnnouncementGoszakupDto> Announcements { get; set; }
        public DbSet<LotGoszakupDto> Lots { get; set; }
        public DbSet<TenderDocumentGoszakupDto> Documents { get; set; }
        public DbSet<AnnouncementFileGoszakupDto> AnnouncementFiles { get; set; }
        public DbSet<LotFileGoszakupDto> LotFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AnnoId)
                .HasPrincipalKey(x => x.Id);
            
            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasMany(x => x.Files)
                .WithOne()
                .HasForeignKey(x => x.AnnoId)
                .HasPrincipalKey(x => x.Id);
            
            modelBuilder.Entity<LotGoszakupDto>()
                .HasMany(x => x.Files)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}