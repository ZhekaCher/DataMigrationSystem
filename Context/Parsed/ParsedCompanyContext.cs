using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCompanyContext  : ParsedContext
    {
        public DbSet<CompanyDto> CompanyDtos { get; set; }    
    }
}