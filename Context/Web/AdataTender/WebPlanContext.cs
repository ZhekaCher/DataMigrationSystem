using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.AdataTender
{
    public partial class WebPlanContext:WebAdataTenderContext
    {
        public DbSet<Plan> Plans { get; set; }
    }
}