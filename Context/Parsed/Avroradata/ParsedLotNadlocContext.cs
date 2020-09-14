using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedLotNadlocContext : ParsedAvroradataContext
    {
        public DbSet<LotNadlocDto> NadlocLotsDtos { get; set; }
       
    }
}