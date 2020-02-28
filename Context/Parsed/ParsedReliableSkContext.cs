using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedReliableSkContext : ParsedContext
    {
        
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<ReliableSkDto> ReliableSkDtos { get; set; }
    }
}