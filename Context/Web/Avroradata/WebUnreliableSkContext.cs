using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnreliableSkContext: WebAvroradataContext
    {
        public DbSet<UnreliableSk> UnreliableSks { get; set; }
    }
}