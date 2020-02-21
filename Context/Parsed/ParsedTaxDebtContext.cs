using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTaxDebtContext: DbContext

    {
        public ParsedTaxDebtContext(DbContextOptions<WebTaxDebtContext> options)
            : base(options)
        {
            
        }

        public ParsedTaxDebtContext()
        {
            
        }

        public DbSet<TaxDebtDto> TaxDebts { get; set; }
        public DbSet<TaxDebtBccDto> TaxDebtBccs { get; set; }
        public DbSet<TaxDebtPayerDto> TaxDebtPayers { get; set; }
        public DbSet<TaxDebtOrgDto> TaxDebtOrgs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxDebtOrgDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtPayerDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtBccDto>().HasKey(x => new {x.CharCode, x.IinBin, x.Bcc});
            modelBuilder.Entity<TaxDebtDto>().HasMany(x => x.TaxDebtOrgs).WithOne(x=>x.TaxDebt).HasForeignKey(x=> x.IinBin).HasPrincipalKey(x=>x.IinBin).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtOrgDto>().HasMany(x => x.TaxDebtPayers).WithOne(x=>x.TaxDebtOrg).HasForeignKey(x=> new {x.CharCode, x.HeadIinBin}).HasPrincipalKey(x=>new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtPayerDto>().HasMany(x => x.TaxDebtBccs).WithOne(x=>x.TaxDebtPayer).HasForeignKey(x=> new {x.CharCode, x.IinBin}).HasPrincipalKey(x=> new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
    }
}        