using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedEtsTenderContext: ParsedAvroradataContext
    {
        public DbSet<AnnouncementEtsTenderDto> AnnouncementEtsTenderDtos { get; set; }
        public DbSet<LotEtsTenderDto> LotEtsTenderDtos { get; set; }
        public DbSet<PurchasingPositionsEtsTenderDto> PurchasingPositionsEtsTenderDtos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnnouncementEtsTenderDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.SourceCode)
                .HasPrincipalKey(x => x.SourceCode);
            modelBuilder.Entity<AnnouncementEtsTenderDto>()
                .HasMany(x => x.Positions)
                .WithOne()
                .HasForeignKey(x => x.SourceCode)
                .HasPrincipalKey(x => x.SourceCode);
        }
    }
}