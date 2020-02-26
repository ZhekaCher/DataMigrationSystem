using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedUnreliableSkContext : ParsedContext
    {
        public DbSet<UnreliableSkDto> UnreliableSkDtos { get; set; }
    }
}