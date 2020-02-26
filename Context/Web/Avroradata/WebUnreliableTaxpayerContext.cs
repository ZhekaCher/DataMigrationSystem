using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebUnreliableTaxpayerContext: WebContext
    {
        
        public DbSet<UnreliableTaxpayer> UnreliableTaxpayers { get; set; }    
    }
}