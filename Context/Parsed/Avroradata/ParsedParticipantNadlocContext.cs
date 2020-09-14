using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedParticipantNadlocContext : ParsedAvroradataContext
    {
        public DbSet<ParticipantNadlocDto> ParticipantNadlocDtos { get; set; }
        public DbSet<CustomerNadlocDto> CustomersNadlocDtos { get; set; }
        public DbSet<SupplierNadlocDto> SupplierNadlocDtos { get; set; }
        public DbSet<AnnouncementNadlocDto> AnnouncementNadlocDtos { get; set; }
    }
}