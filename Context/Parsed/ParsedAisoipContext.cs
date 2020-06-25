using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using AisoipList = DataMigrationSystem.Models.Parsed.AisoipList;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedAisoipContext : ParsedContext
    {
        public static DbSet<Aisoip> AisoipDtos { get; set; }
        public DbSet<AisoipList> AisoipLists { get; set; }
    }
}