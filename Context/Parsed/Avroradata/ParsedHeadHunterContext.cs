using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedHeadHunterContext : ParsedAvroradataContext
    {
        public DbSet<CompanyHhDto> CompanyHhDtos { get; set; }
        public DbSet<VacancyHhDto> VacancyHhDtos { get; set; }
        public DbSet<CompBinhhDto> CompBinhhDtos { get; set; }
        public DbSet<HhResumeDto> HhResumeDtos { get; set; }
        public DbSet<HhResumeBinDto> HhResumeBinDtos { get; set; }
    }
}