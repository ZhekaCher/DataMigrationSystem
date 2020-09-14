using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedSamrukContext : ParsedAvroradataContext
    {
        public DbSet<SamrukAdvertDto> SamrukAdverts { get; set; }
        public DbSet<SamrukFilesDto> SamrukFiles { get; set; }
        public DbSet<SamrukLotsDto> Lots { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamrukAdvertDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<SamrukAdvertDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
            modelBuilder.Entity<SamrukLotsDto>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.LotId);
        }
    }
}