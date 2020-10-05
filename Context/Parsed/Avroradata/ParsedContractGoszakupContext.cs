using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:52:27
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицами контрактов госзакупа
    /// </summary>
    public class ParsedContractGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<ContractGoszakupDto> Contracts { get; set; }
        public DbSet<ContractUnitGoszakupDto> Units { get; set; }
        public DbSet<PlanGoszakupDto> Plans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractGoszakupDto>()
                .HasMany(x => x.Units)
                .WithOne()
                .HasForeignKey(x => x.ContractId)
                .HasPrincipalKey(x => x.Id);
            
            modelBuilder.Entity<ContractUnitGoszakupDto>()
                .HasOne(x => x.Plan)
                .WithOne(x => x.Unit)
                .HasForeignKey<PlanGoszakupDto>(x => x.ContractUnitId)
                .HasPrincipalKey<ContractUnitGoszakupDto>(x => x.Id);
        }
    }
}