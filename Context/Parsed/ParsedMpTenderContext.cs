using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedMpTenderContext : ParsedContext
    {
        public DbSet<MpTenderDto> MpTender { get; set; }

        public DbSet<MpTenderLotsDto> Lots { get; set; }

        public DbSet<MpTenderFilesDto> MpTenderFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MpTenderDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<MpTenderDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<MpTenderLotsDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.LotId);
        }
    }
}