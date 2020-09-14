using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTop100GoszakupContext : ParsedAvroradataContext
    {
        public DbSet<Top100SuppliersGoszakupDto> Top100SuppliersgoszakupDtos { get; set; }    
        public DbSet<Top100CustomersGoszakupDto> Top100CustomersGoszakupDtos { get; set; }
        
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}