namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    using DataMigrationSystem.Models.Parsed.Avroradata.EtbTender;
    using Microsoft.EntityFrameworkCore;
    public class ParsedEtbTenderContext : ParsedAvroradataContext
    {
        public DbSet<EtbTenderDto> EtbTenders { get; set; }
        public DbSet<EtbLotDto> EtbLots { get; set; }
        public DbSet<EtbDetailDto> EtbDetails { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EtbTenderDto>()
                .HasKey(o => new { o.IdGroup });
            modelBuilder.Entity<EtbLotDto>()
                .HasKey(o => new { o.IdLot, o.IdGroup });
            modelBuilder.Entity<EtbDetailDto>()
                .HasKey(o => new { o.IdGroup, o.IdLot, o.IdDetail });
            modelBuilder.Entity<EtbTenderDto>()
                .HasMany(x => x.EtbLots)
                .WithOne()
                .HasForeignKey(x => new { x.IdGroup })
                .HasPrincipalKey(x => new { x.IdGroup });
            modelBuilder.Entity<EtbLotDto>()
                .HasMany(x => x.EtbDetails)
                .WithOne()
                .HasForeignKey(x => new { x.IdLot, x.IdGroup })
                .HasPrincipalKey(x => new { x.IdLot, x.IdGroup });
        }
    }
}