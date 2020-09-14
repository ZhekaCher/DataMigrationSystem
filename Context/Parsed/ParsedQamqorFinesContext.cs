using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedQamqorFinesContext : ParsedAvroradataContext
    {
        public DbSet<QamqorFinesDto> QamqorFinesDtos { get; set; }
    }
}