using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCompanyContext: DbContext
    {
        public WebCompanyContext(DbContextOptions<WebCompanyContext> options)
            : base(options)
        {
            
        }

        public WebCompanyContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }

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