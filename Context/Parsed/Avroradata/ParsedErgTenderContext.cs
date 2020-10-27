using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedErgTenderContext : ParsedAvroradataContext
    {
        public DbSet<ErgTenderDto.ErgBiddingConcourse> ErgBiddingConcourse { get; set; }
        public DbSet<ErgTenderDto.ErgBiddingConcoursePositions> ErgBiddingConcoursePositions { get; set; }
    }
}