using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedTaxDebtContext : ParsedAvroradataContext

    {

        public DbSet<TaxDebtDto> TaxDebts { get; set; }
        public DbSet<TaxDebtBccDto> TaxDebtBccs { get; set; }
        public DbSet<TaxDebtPayerDto> TaxDebtPayers { get; set; }
        public DbSet<TaxDebtOrgDto> TaxDebtOrgs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxDebtOrgDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtPayerDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtBccDto>().HasKey(x => new {x.CharCode, x.IinBin, x.Bcc});
            modelBuilder.Entity<TaxDebtDto>().HasMany(x => x.TaxDebtOrgs).WithOne().HasForeignKey(x=> x.IinBin).HasPrincipalKey(x=>x.IinBin);
            modelBuilder.Entity<TaxDebtOrgDto>().HasMany(x => x.TaxDebtPayers).WithOne().HasForeignKey(x=> new {x.CharCode, x.HeadIinBin}).HasPrincipalKey(x=>new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtPayerDto>().HasMany(x => x.TaxDebtBccs).WithOne().HasForeignKey(x=> new {x.CharCode, x.IinBin}).HasPrincipalKey(x=> new {x.CharCode, x.IinBin});
        }
       
    }
}        