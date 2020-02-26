using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnreliableSkContext: WebContext
    {
        public DbSet<UnreliableSk> UnreliableSks { get; set; }
    }
}