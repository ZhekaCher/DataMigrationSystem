using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public partial class ParsedPlanGoszakupContext:ParsedAvroradataContext
    {
        public DbSet<PlanGoszakupDto> PlanGoszakupDtos { get; set; }
    }
}