using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedESaudaTenderContext : ParsedAvroradataContext
    {
        public DbSet<EsaudaTenderDto> EsaudaTenderDtos { get; set; }
    }
}