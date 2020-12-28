using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedKompraBinParserContext : ParsedAvroradataContext
    {
        public DbSet<KompraBinParserDto> KompraBinParserDto { get; set; }
    }
}