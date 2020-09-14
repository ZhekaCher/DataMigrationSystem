using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedProducerSkContext : ParsedAvroradataContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<ProducerSkDto> ProducerSkDtos { get; set; }
        public DbSet<ProducerProductsDto> ProducerProductsDtos { get; set; }
    }
}