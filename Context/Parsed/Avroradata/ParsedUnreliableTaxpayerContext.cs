using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedUnreliableTaxpayerContext : ParsedAvroradataContext
    {
        public DbSet<UnreliableTaxpayerDto> UnreliableTaxpayerDtos { get; set; }    
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }    
    }
}