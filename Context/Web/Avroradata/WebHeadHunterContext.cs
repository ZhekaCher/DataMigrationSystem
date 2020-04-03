using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebHeadHunterContext : WebContext
    {
        public DbSet<CompanyHh> CompanyHhs { get; set; }
        public DbSet<VacancyHh> VacancyHhs { get; set; }

    }
}