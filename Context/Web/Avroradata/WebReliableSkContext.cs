using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebReliableSkContext: WebContext
    {
        
        public DbSet<ReliableSk> ReliableSks { get; set; }
    }
}