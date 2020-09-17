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
    public class WebContractContext : WebAdataTenderContext
    {
        public DbSet<AdataContract> Contracts { get; set; }    
        public DbSet<ContractStatuses> ContractStatuses { get; set; }
        public DbSet<Method> ContractMethods { get; set; }
        public DbSet<ContractTypes> ContractTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}