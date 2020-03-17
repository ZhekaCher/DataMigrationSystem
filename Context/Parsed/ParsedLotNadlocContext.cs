using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedLotNadlocContext : ParsedContext
    {
        public DbSet<LotNadlocDto> NadlocLotsDtos { get; set; }
       
    }
}