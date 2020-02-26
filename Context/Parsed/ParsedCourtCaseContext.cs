using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCourtCaseContext: ParsedContext
    {
        public DbSet<CourtCaseDto> CourtCaseDtos { get; set; }    
        public DbSet<CourtCaseEntityDto> CourtCaseEntityDtos { get; set; }    
        public DbSet<CompanyDto> CompanyDtos { get; set; }
    }
}