using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebHeadHunterContext : WebContext
    {
        public DbSet<CompanyHh> CompanyHhs { get; set; }
        public DbSet<VacancyHh> VacancyHhs { get; set; }
        public DbSet<CompBinhh> CompBinhhs { get; set; }
        public DbSet<HhResume> HhResumes { get; set; }
        public DbSet<HhResumeBin> HhResumeBins { get; set; }

    }
}