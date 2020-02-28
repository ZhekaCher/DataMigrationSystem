using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedUnreliableSkContext : ParsedContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<UnreliableSkDto> UnreliableSkDtos { get; set; }
    }
}