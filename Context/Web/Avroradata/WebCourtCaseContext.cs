using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCourtCaseContext: WebContext
    {

        public DbSet<CourtCase> CourtCases { get; set; }    
        public DbSet<CourtCaseDocumentType> DocumentTypes { get; set; }     
        public DbSet<CourtCaseType> CaseTypes { get; set; }
        public DbSet<Court> Courts { get; set; }    
        public DbSet<CompanyCourtCaseEntity> CompanyCourtCaseEntities { get; set; }
        public DbSet<CourtCaseCategory> CourtCaseCategories { get; set; }
    }
}