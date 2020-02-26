using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebDisabilitiesOrgSkContext: WebContext
    {
        
        public DbSet<DisabilitiesOrgSk> DisabilitiesOrgSk { get; set; }
    }
}