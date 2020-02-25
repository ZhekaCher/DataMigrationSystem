using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedUnreliableSkContext:DbContext
    {
        public ParsedUnreliableSkContext(DbContextOptions<ParsedUnreliableSkContext> options)
            : base(options) { }
        public ParsedUnreliableSkContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        public DbSet<UnreliableSkDto> UnreliableSkDtos { get; set; }
    }
}