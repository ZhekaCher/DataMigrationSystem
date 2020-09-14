using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedPedophilesContext : ParsedAvroradataContext
    {
        public DbSet<IndividualIin> IndividualIins { get; set; }   
        public DbSet<PedophileDto> PedophileDtos { get; set; }
    }
}