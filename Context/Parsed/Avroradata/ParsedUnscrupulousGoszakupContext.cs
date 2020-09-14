using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:02:33
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'unscrupulouses_goszakup'
    /// </summary>
    public class ParsedUnscrupulousGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<UnscrupulousGoszakupDto> UnscrupulousGoszakupDtos { get; set; }
        public DbSet<RnuReferenceGoszakupDto> RnuReferenceGoszakupDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnscrupulousGoszakupDto>().HasMany(x => x.RnuReferenceGoszakupDtos).WithOne()
                .HasForeignKey(x => x.SupplierBiin).HasPrincipalKey(x => x.SupplierBiin);
        }
    }
}