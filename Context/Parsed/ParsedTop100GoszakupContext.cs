using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTop100GoszakupContext: DbContext
    {
        
        public DbSet<Top100SuppliersGoszakupDto> Top100SuppliersgoszakupDtos { get; set; }    
        public DbSet<Top100CustomersGoszakupDto> Top100CustomersGoszakupDtos { get; set; }    
        public ParsedTop100GoszakupContext(DbContextOptions<ParsedTop100GoszakupContext> options)
            : base(options)
        {

        }

        public ParsedTop100GoszakupContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
    }
}