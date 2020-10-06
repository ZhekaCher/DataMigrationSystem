using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedKazpatentContext : ParsedAvroradataContext
    {
        public DbSet<KazpatentDto> KazpatentDtos { get; set; }
        public DbSet<KazPatentOwnerDto> KazPatentOwnerDtos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<KazpatentDto>()
                .HasMany(x => x.KazPatentOwnerDtos)
                .WithOne()
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.PatentId);
        }
    }
}