using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:52:27
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'contract_goszakup'
    /// </summary>
    public class ParsedContractGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<ContractGoszakupDto> ContractGoszakupDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContractGoszakupDto>()
                .HasOne(x => x.RefStatus)
                .WithOne()
                .HasForeignKey<ContractGoszakupDto>(x => x.RefContractStatusId)
                .HasPrincipalKey<RefContractStatusGoszakupDto>(x => x.Id);
            
            modelBuilder.Entity<ContractGoszakupDto>()
                .HasOne(x => x.RefTradeMethod)
                .WithOne()
                .HasForeignKey<ContractGoszakupDto>(x => x.FaktTradeMethodsId)
                .HasPrincipalKey<RefTradeMethodGoszakupDto>(x => x.Id);
            
            modelBuilder.Entity<ContractGoszakupDto>()
                .HasOne(x => x.RefType)
                .WithOne()
                .HasForeignKey<ContractGoszakupDto>(x => x.RefContractTypeId)
                .HasPrincipalKey<RefContractTypeGoszakupDto>(x => x.Id);
        }
    }
}