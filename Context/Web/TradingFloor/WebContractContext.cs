using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.TradingFloor
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:56:55
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'contract' и 'contract_goszakup'
    /// </summary>
    public class WebContractContext : WebContext
    {
        
        public DbSet<Contract> Contracts { get; set; }    
        public DbSet<ContractGoszakup> ContractsGoszakup { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trading_floor");
        }
    }
}