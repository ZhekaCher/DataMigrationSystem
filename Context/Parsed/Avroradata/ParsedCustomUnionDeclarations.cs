using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedCustomUnionDeclarations : ParsedAvroradataContext
    {
        public DbSet<CustomUnionDeclarationsDto> CustomUnionDeclarationsDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomUnionDeclarationsDto>()
                .HasMany(x => x.CustomUnionDeclarationsAdDto)
                .WithOne()
                .HasForeignKey(x => x.Declarations)
                .HasPrincipalKey(x => x.Id);
        }
    }

   
}