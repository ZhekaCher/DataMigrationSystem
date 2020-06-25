using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebAisoipContext : WebContext
    {
        public DbSet<Aisoip> Aisoip { get; set; }
        public DbSet<AisoipList> AisoipLists{ get; set; }
    }
}