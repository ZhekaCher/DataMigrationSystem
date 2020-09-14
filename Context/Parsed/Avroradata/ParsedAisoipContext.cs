using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedAisoipContext : ParsedAvroradataContext
    {
        public DbSet<AisoipDto> AisoipDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AisoipDto>()
                .HasMany(x => x.AisoipDetailsDtos)
                .WithOne()
                .HasForeignKey(x => x.AresId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}