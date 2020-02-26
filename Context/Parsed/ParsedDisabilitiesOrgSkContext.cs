using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedDisabilitiesOrgSkContext : DbContext
    {
        public DbSet<DisabilitiesOrgSkDto> DisabilitiesOrgSkDtos { get; set; }
    }
}