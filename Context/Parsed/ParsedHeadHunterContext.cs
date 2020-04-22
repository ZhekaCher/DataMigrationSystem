using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedHeadHunterContext : ParsedContext
    {
        public DbSet<CompanyHhDto> CompanyHhDtos { get; set; }
        public DbSet<VacancyHhDto> VacancyHhDtos { get; set; }
        public DbSet<CompBinhhDto> CompBinhhDtos { get; set; }
    }
}