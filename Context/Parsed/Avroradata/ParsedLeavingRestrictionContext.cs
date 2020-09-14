using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedLeavingRestrictionContext : ParsedAvroradataContext
    {
        public DbSet<LeavingRestrictionDto> LeavingRestrictionDtos { get; set; }    
    }
}