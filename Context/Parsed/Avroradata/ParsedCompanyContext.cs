using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedCompanyContext  : ParsedAvroradataContext
    {
        public DbSet<CompanyDto> CompanyDtos { get; set; }    
    }
}