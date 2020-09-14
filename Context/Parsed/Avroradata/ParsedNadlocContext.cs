using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedNadlocContext : ParsedAvroradataContext
    {
        public DbSet<AnnouncementNadlocDto> AnnouncementNadlocDtos { get; set; }
        public DbSet<LotNadlocDto> LotNadlocDtos { get; set; }
        // public DbSet<CompanyBinDto> Companies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnouncementNadlocDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.TenderId)
                .HasPrincipalKey(x => x.FullId);
        }
    }
}