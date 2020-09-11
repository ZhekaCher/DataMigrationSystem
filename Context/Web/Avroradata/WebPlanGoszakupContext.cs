using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public partial class WebPlanGoszakupContext:WebContext
    {
        public DbSet<PlanGoszakup> PlansGoszakup { get; set; }
    }
}