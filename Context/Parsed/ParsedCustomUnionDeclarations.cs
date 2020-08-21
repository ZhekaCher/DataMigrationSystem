using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedCustomUnionDeclarations : ParsedContext
    {
        public DbSet<CustomUnionDeclarationsDto> CustomUnionDeclarationsDtos { get; set; }
        public DbSet<CustomUnionDeclarationsAdDto> CustomUnionDeclarationsAdDtos { get; set; }
    }

   
}