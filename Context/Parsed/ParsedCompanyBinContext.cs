using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCompanyBinContext : ParsedContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}