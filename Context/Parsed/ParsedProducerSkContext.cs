using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedProducerSkContext:DbContext
    {
        public ParsedProducerSkContext(DbContextOptions<ParsedPedophilesContext> options)
            :base(options){}
        public ParsedProducerSkContext(){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("");
        }
        public DbSet<ProducerSkDto> ProducerSkDtos { get; set; }
    }
}