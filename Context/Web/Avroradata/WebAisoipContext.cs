using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebAisoipContext : WebAvroradataContext
    {
        public DbSet<Aisoip> Aisoip { get; set; }
        public DbSet<AisoipList> AisoipLists{ get; set; }
        public DbSet<AisoipDetails> AisoipDetails{ get; set; }
    }
}