using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedUnreliableSkContext : ParsedAvroradataContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<UnreliableSkDto> UnreliableSkDtos { get; set; }
    }
}