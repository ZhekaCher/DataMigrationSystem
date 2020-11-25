using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedMitworkTenderContext: ParsedAvroradataContext
    {
        public DbSet<MitworkAdvertDto> MitworkAdvert { get; set; }
        public DbSet<MitworkLotDto> Lots { get; set; }
        public DbSet<MitworkFileDto> MitworkFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MitworkAdvertDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<MitworkAdvertDto>()
                .HasMany(x => x.AdvertDocumentations)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<MitworkLotDto>()
                .HasMany(x => x.LotDocumentations)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.LotId);
        }
    }
}