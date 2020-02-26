using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedUnreliableTaxpayerContext : ParsedContext
    {
        public DbSet<UnreliableTaxpayerDto> UnreliableTaxpayerDtos { get; set; }    
    }
}