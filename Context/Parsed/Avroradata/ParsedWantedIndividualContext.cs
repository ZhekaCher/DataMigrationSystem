using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedWantedIndividualContext : ParsedAvroradataContext 
    {
        public DbSet<WantedIndividualDto> WantedIndividualDtos { get; set; }
    }
}