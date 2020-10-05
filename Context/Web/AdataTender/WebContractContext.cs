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
        public DbSet<AdataContracts> Contracts { get; set; }
        public DbSet<ContractStatus> ContractStatuses { get; set; }
        public DbSet<Measure> Measures { get; set; }
        public DbSet<TruCode> TruCodes { get; set; }
        public DbSet<ContractType> Types { get; set; }
        public DbSet<ContractYearType> YearTypes { get; set; }
        public DbSet<ContractAgrForm> AgrForms { get; set; }
        public DbSet<Method> Methods { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<ContractUnit> ContractUnits { get; set; }
        public DbSet<AdataPlan> Plans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}