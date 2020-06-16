using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;


namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedSamrukContext : ParsedContext
    {
        public DbSet<SamrukAdvertDto> SamrukAdverts { get; set; }
        public DbSet<SamrukFilesDto> SamrukFiles { get; set; }
        public DbSet<SamrukLotsDto> Lots { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamrukAdvertDto>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AdvertId)
                .HasPrincipalKey(x => x.AdvertId);
        }
    }
}