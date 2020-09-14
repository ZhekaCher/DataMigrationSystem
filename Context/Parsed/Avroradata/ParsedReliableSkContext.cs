using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedReliableSkContext : ParsedAvroradataContext
    {
        
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<ReliableSkDto> ReliableSkDtos { get; set; }
    }
}