using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedNdsDetailsContext:ParsedContext
    {
        public DbSet<NdsDetailsDto> NdsDetailsDtos { get; set; }
    }
}