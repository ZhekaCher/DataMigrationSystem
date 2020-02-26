using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedProducerSkContext : ParsedContext
    {
        public DbSet<ProducerSkDto> ProducerSkDtos { get; set; }
    }
}