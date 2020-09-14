using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedCourtCaseContext: ParsedAvroradataContext
    {
        public DbSet<CourtCaseDto> CourtCaseDtos { get; set; }    
        public DbSet<CourtCaseEntityDto> CourtCaseEntityDtos { get; set; }    
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}