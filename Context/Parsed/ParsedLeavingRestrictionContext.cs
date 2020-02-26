using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedLeavingRestrictionContext : ParsedContext
    {
        public DbSet<LeavingRestrictionDto> LeavingRestrictionDtos { get; set; }    
    }
}