using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedSamrukParticipantsContext:ParsedAvroradataContext
    {
        public DbSet<SamrukParticipantsDto> SamrukParticipantsDtos { get; set; }
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }    

    }
}