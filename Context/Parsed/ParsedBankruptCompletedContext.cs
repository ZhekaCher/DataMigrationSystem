using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedBankruptCompletedContext : ParsedContext
    {
        public DbSet<BankruptCompletedDto> BankruptCompletedDtos { get; set; }    
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
    }
}