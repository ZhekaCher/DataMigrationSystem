using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedParticipantNadlocContext : ParsedContext
    {
        public DbSet<ParticipantNadlocDto> ParticipantNadlocDtos { get; set; }
        public DbSet<CustomerNadlocDto> CustomersNadlocDtos { get; set; }
        public DbSet<SupplierNadlocDto> SupplierNadlocDtos { get; set; }
        public DbSet<AnnouncementNadlocDto> AnnouncementNadlocDtos { get; set; }
    }
}