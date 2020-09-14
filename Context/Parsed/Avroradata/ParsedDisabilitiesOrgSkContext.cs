using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedDisabilitiesOrgSkContext : ParsedAvroradataContext
    {
        public DbSet<DisabilitiesOrgSkDto> DisabilitiesOrgSkDtos { get; set; }
        public DbSet<DisabilitiesOrganizationsProductsSkDto> DisabilitiesOrganizationsProductsSkDtos { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}