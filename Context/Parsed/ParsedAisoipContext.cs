using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedAisoipContext : ParsedContext
    {
        public DbSet<Aisoip> AisoipDtos { get; set; }
        public DbSet<AisoipList> AisoipLists { get; set; }
    }
}