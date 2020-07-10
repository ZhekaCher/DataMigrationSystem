using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedAisoipContext : ParsedContext
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