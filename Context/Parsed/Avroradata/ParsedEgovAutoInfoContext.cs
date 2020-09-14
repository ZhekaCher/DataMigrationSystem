using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedEgovAutoInfoContext : ParsedAvroradataContext
    {
        public DbSet<EgovAutoInfoDto> EgovAutoInfoDtos { get; set; }
    }
}