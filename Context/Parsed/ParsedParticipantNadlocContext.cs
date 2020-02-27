using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedParticipantNadlocContext : ParsedContext
    {
        public DbSet<ParticipantNadlocDto> ParticipantNadlocDtos { get; set; }
    }
}