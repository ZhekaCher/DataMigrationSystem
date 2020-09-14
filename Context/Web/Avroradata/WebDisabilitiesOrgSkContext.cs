using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebDisabilitiesOrgSkContext: WebAvroradataContext
    {
        public DbSet<DisabilitiesOrgSk> DisabilitiesOrgSk { get; set; }
        public DbSet<DisabilitiesOrganizationsProductsSk> DisabilitiesOrganizationsProductsSks { get; set; }
    }
}