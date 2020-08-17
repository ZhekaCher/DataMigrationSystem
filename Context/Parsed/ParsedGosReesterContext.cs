using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedGosReesterContext:ParsedContext
    {
        public DbSet<GosReesterDto> GosReesterDtos { get; set; }
    }
}