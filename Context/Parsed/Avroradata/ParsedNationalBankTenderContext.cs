using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedNationalBankTenderContext : ParsedAvroradataContext
    {
        public DbSet<NationalBankTenderDto> NationalBankAdvert { get; set; }
        public DbSet<NationalBankFileDto> TenderFiles { get; set; }
        public DbSet<NationalBankTenderLotDto> Lots { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NationalBankTenderDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<NationalBankTenderDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<NationalBankTenderLotDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.LotId);
        }
    }
}