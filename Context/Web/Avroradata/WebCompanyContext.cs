using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCompanyContext : WebContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Kato> Katos { get; set; }
        public DbSet<Krp> Krps { get; set; }
        public DbSet<Oked> Okeds { get; set; }
        public DbSet<CompanyOked> CompaniesOkeds { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyOked>().HasKey(x => new {x.CompanyId, x.OkedId});
            modelBuilder.Entity<Company>().HasMany(x => x.CompanyOkeds).WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId).HasPrincipalKey(x => x.Bin);
        }
    }
}