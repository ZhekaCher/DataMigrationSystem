using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedGosreestrCapitalContext : ParsedAvroradataContext
    {
        public DbSet<GosreestrBinDto> GosreestrBinDtos { get; set; }
        public DbSet<GosreestrCapitalDto> GosreestrCapitalDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GosreestrBinDto>()
                .HasOne(x => x.GosreestrCapitalDto)
                .WithOne()
                .HasPrincipalKey<GosreestrBinDto>(x => x.Bin)
                .HasForeignKey<GosreestrCapitalDto>(x => x.Bin);
        }
    }
}