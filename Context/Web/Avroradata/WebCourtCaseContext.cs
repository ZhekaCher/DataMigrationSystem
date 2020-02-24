using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCourtCaseContext: DbContext
    {
        public WebCourtCaseContext(DbContextOptions<WebCourtCaseContext> options)
            : base(options)
        {
        }

        public WebCourtCaseContext()
        {
            
        }

        public DbSet<CourtCase> CourtCases { get; set; }    
        public DbSet<CourtCaseDocumentType> DocumentTypes { get; set; }     
        public DbSet<CourtCaseType> CaseTypes { get; set; }
        public DbSet<Court> Courts { get; set; }    
        public DbSet<CompanyCourtCaseEntity> CompanyCourtCaseEntities { get; set; }
        public DbSet<CourtCaseCategory> CourtCaseCategories { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}