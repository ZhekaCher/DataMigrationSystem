using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedTaxDebtContext : ParsedContext

    {

        public DbSet<TaxDebtDto> TaxDebts { get; set; }
        public DbSet<TaxDebtBccDto> TaxDebtBccs { get; set; }
        public DbSet<TaxDebtPayerDto> TaxDebtPayers { get; set; }
        public DbSet<TaxDebtOrgDto> TaxDebtOrgs { get; set; }
        public DbSet<CompanyDto> CompanyDtos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxDebtOrgDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtPayerDto>().HasKey(x => new {x.CharCode, x.IinBin});
            modelBuilder.Entity<TaxDebtBccDto>().HasKey(x => new {x.CharCode, x.IinBin, x.Bcc});
            modelBuilder.Entity<TaxDebtDto>().HasMany(x => x.TaxDebtOrgs).WithOne(x=>x.TaxDebt).HasForeignKey(x=> x.IinBin).HasPrincipalKey(x=>x.IinBin).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtOrgDto>().HasMany(x => x.TaxDebtPayers).WithOne(x=>x.TaxDebtOrg).HasForeignKey(x=> new {x.CharCode, x.HeadIinBin}).HasPrincipalKey(x=>new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TaxDebtPayerDto>().HasMany(x => x.TaxDebtBccs).WithOne(x=>x.TaxDebtPayer).HasForeignKey(x=> new {x.CharCode, x.IinBin}).HasPrincipalKey(x=> new {x.CharCode, x.IinBin}).OnDelete(DeleteBehavior.Cascade);
        }
       
    }
}        