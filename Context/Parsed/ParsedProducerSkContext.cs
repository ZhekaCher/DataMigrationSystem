using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedProducerSkContext : ParsedContext
    {
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        public DbSet<ProducerSkDto> ProducerSkDtos { get; set; }
        public DbSet<ProducerProductsDto> ProducerProductsDtos { get; set; }
    }
}