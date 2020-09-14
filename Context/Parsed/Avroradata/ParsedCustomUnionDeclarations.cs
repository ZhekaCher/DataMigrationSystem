using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedCustomUnionDeclarations : ParsedAvroradataContext
    {
        public DbSet<CustomUnionDeclarationsDto> CustomUnionDeclarationsDtos { get; set; }
        public DbSet<CustomUnionDeclarationsAdDto> CustomUnionDeclarationsAdDtos { get; set; }
    }

   
}