using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedUnscrupulousSuppliers : ParsedAvroradataContext
    {
        public DbSet<UnscrupulousSuppliersDto> UnscrupulousSuppliersesDto { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyBinDto>()
                .HasMany(x => x.UnscrupulousSuppliersDtos)
                .WithOne()
                .HasPrincipalKey(x => x.Code)
                .HasForeignKey(x => x.IinBiin);
        }
    }
}