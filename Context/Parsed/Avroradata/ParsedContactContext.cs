using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedContactContext : ParsedAvroradataContext
    {
        public DbSet<ContactDto> ContactsDtos { get; set; }

    }
}