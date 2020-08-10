using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;


namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedGoszakupContext : ParsedContext
    {
        public DbSet<AnnouncementGoszakupDto> AnnouncementGoszakupDtos { get; set; }
        public DbSet<LotGoszakupDto> LotGoszakupDtos { get; set; }
        public DbSet<PlanGoszakupDto> PlanGoszakupDtos { get; set; }
        public DbSet<TenderDocumentGoszakupDto> TenderDocumentsGoszakup { get; set; }
        public DbSet<RefTradeMethodGoszakupDto> RefTradeMethodGoszakupDtos { get; set; }
        public DbSet<RefLotStatusGoszakupDto> RefLotStatusGoszakupDtos { get; set; }
        public DbSet<RefBuyStatusGoszakupDto> RefBuyStatusGoszakupDtos { get; set; }
        public DbSet<RefUnitGoszakupDto> RefUnitGoszakupDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.TrdBuyNumberAnno)
                .HasPrincipalKey(x => x.NumberAnno);
            
            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasOne(x => x.RefTradeMethod)
                .WithOne()
                .HasForeignKey<AnnouncementGoszakupDto>(x => x.RefTradeMethodsId)
                .HasPrincipalKey<RefTradeMethodGoszakupDto>(x => x.Id);
            modelBuilder.Entity<AnnouncementGoszakupDto>()
                .HasOne(x => x.RefBuyStatus)
                .WithOne()
                .HasForeignKey<AnnouncementGoszakupDto>(x => x.RefBuyStatusId)
                .HasPrincipalKey<RefBuyStatusGoszakupDto>(x => x.Id);
            
            
            // Lots
            modelBuilder.Entity<LotGoszakupDto>()
                .HasOne(x => x.RefTradeMethod)
                .WithOne()
                .HasForeignKey<LotGoszakupDto>(x => x.RefTradeMethodsId)
                .HasPrincipalKey<RefTradeMethodGoszakupDto>(x => x.Id);
            modelBuilder.Entity<LotGoszakupDto>()
                .HasOne(x => x.RefLotStatus)
                .WithOne()
                .HasForeignKey<LotGoszakupDto>(x => x.RefLotStatusId)
                .HasPrincipalKey<RefLotStatusGoszakupDto>(x => x.Id);
        }
    }
}