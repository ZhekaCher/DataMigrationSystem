using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedEgovAutoInfoContext : ParsedContext
    {
        public DbSet<EgovAutoInfoDto> EgovAutoInfoDtos { get; set; }
    }
}