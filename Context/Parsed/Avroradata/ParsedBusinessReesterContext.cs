using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedBusinessReesterContext : ParsedAvroradataContext
    {
        public DbSet<BusinessReesterDto> BusinessReesterDtos { get; set; }
    }
}