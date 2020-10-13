using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedKaspiAuctionContext : ParsedAvroradataContext
    {
        public DbSet<KaspiAuctionDto> KaspiAuctionDtos { get; set; }
    }
}