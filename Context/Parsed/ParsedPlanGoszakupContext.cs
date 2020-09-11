using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public partial class ParsedPlanGoszakupContext:ParsedContext
    {
        public DbSet<PlanGoszakupDto> PlanGoszakupDtos { get; set; }
    }
}