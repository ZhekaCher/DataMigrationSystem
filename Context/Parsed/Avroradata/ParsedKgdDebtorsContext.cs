using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedKgdDebtorsContext : ParsedAvroradataContext
    {
        public DbSet<KgdDebtorsDto> KgdDebtorsDtos { get; set; }
        public DbSet<KgdDebtorsAgentsDto> KgdDebtorsAgentsDtos { get; set; }
        public DbSet<KgdDebtorsCustomersDto> KgdDebtorsCustomersDtos { get; set; }
    }
}