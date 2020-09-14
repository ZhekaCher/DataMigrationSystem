using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebNdsDetailsContext:WebAvroradataContext
    {
        public DbSet<NdsDetails> NdsDetailses { get; set; }
        public DbSet<NdsDetailReason> NdsDetailReasons { get; set; }
    }
}