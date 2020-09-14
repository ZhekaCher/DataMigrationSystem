using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebLeavingRestrictionContext: WebAvroradataContext
    {

        public DbSet<LeavingRestriction> LeavingRestrictions { get; set; }
    }
}