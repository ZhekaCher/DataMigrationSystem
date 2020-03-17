using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedEnforcementDebtContext : ParsedContext
    {
        public DbSet<EnforcementDebtDto> EgovEnforcementDebtDtos { get; set; }
    }
}