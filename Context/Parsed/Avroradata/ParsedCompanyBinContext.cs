using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedCompanyBinContext : ParsedAvroradataContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}