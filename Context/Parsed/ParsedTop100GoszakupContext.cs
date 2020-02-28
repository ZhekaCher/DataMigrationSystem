using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTop100GoszakupContext : ParsedContext
    {
        public DbSet<Top100SuppliersGoszakupDto> Top100SuppliersgoszakupDtos { get; set; }    
        public DbSet<Top100CustomersGoszakupDto> Top100CustomersGoszakupDtos { get; set; }
        
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}