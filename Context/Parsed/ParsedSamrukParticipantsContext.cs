using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedSamrukParticipantsContext:ParsedContext
    {
        public DbSet<SamrukParticipantsDto> SamrukParticipantsDtos { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }    

    }
}