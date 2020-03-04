using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedDisabilitiesOrgSkContext : ParsedContext
    {
        public DbSet<DisabilitiesOrgSkDto> DisabilitiesOrgSkDtos { get; set; }
        public DbSet<DisabilitiesOrganizationsProductsSkDto> DisabilitiesOrganizationsProductsSkDtos { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}