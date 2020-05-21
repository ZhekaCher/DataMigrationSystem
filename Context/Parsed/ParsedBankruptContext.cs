using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedBankruptContext : ParsedContext
    {
        public DbSet<BankruptAtStageDto> BankruptAtStageDtos { get; set; }
        public DbSet<BankruptCompletedDto> BankruptCompletedDtos { get; set; }    
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}