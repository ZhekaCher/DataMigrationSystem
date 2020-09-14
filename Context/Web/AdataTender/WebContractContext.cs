using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.AdataTender
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:56:55
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'contract' и 'contract_goszakup'
    /// </summary>
    public class WebContractContext : WebAvroradataTenderContext
    {
        
        public DbSet<Contract> Contracts { get; set; }    
        public DbSet<ContractGoszakup> ContractsGoszakup { get; set; }    
        public DbSet<STradingFloor> STradingFloors { get; set; }    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("trading_floor");
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = adata_tender; Integrated Security=true; Pooling=true;");
        }
    }
}