using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebKazpatentContext : WebAvroradataContext
    {
        public DbSet<Kazpatent> Kazpatents { get; set; }
        public DbSet<KazPatentOwner> KazPatentOwners { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Kazpatent>()
                .HasMany(x => x.KazPatentOwners)
                .WithOne()
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.PatentId);
        }
    }
}