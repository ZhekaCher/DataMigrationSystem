using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedBankruptAtStageContext : ParsedContext
    {
        public DbSet<BankruptAtStageDto> BankruptAtStageDtos { get; set; }    
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }   
    }
}