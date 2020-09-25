using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebGosreestrCapitalContext : WebAvroradataContext
    {
        public DbSet<GosreestrBin> GosreestrBins { get; set; }
        public DbSet<GosreestrCapital> GosreestrCapitals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GosreestrBin>()
                .HasOne(x => x.GosreestrCapital)
                .WithOne()
                .HasPrincipalKey<GosreestrBin>(x => x.Bin)
                .HasForeignKey<GosreestrCapital>(x => x.Bin);
        }
    }
}