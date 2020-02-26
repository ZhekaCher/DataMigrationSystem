using Microsoft.EntityFrameworkCore;
using SomeNamespace;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCompanyBinContext : ParsedContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}