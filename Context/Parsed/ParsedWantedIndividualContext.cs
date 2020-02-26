using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedWantedIndividualContext : ParsedContext 
    {
        public DbSet<WantedIndividualDto> WantedIndividualDtos { get; set; }
    }
}