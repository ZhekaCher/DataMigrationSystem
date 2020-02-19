using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class WebTaxDebtContext: DbContext

    {
        public WebTaxDebtContext(DbContextOptions<WebTaxDebtContext> options)
            : base(options)
        {
            
        }

        public WebTaxDebtContext()
        {
            
        }

        public DbSet<TaxDebt> TaxDebts { get; set; }
        public DbSet<TaxDebtBcc> TaxDebtBccs { get; set; }
        public DbSet<TaxDebtPayer> TaxDebtPayers { get; set; }
        public DbSet<TaxDebtOrg> TaxDebtOrgs { get; set; }
        public DbSet<TaxDebtBccName> TaxDebtBccNames { get; set; }
        public DbSet<TaxDebtOrgName> TaxDebtOrgNames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxDebtOrg>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtPayer>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtBcc>().HasKey(x => new {x.CharCode, x.IinBin, x.Bcc});
            modelBuilder.Entity<TaxDebt>().HasMany(x => x.TaxDebtOrgs).WithOne(x=>x.TaxDebt).HasForeignKey(x=> x.IinBin).HasPrincipalKey(x=>x.IinBin).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtOrg>().HasMany(x => x.TaxDebtPayers).WithOne(x=>x.TaxDebtOrg).HasForeignKey(x=> new {x.CharCode, x.HeadIinBin}).HasPrincipalKey(x=>new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtPayer>().HasMany(x => x.TaxDebtBccs).WithOne(x=>x.TaxDebtPayer).HasForeignKey(x=> new {x.CharCode, x.IinBin}).HasPrincipalKey(x=> new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = avroradata; Integrated Security=true; Pooling=true;");
        }
    }
}        