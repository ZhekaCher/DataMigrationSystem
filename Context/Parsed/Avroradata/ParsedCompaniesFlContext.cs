using DataMigrationSystem.Models.Parsed.Avroradata.CompaniesFl;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    using DataMigrationSystem.Models.Parsed.Avroradata.EtbTender;
    using Microsoft.EntityFrameworkCore;
    
    public class ParsedCompaniesFlContext : ParsedAvroradataContext
    {
            public DbSet<CompaniesFlDto> CompaniesFlDtos { get; set; }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<CompaniesFlDto>()
                    .ToTable("company_from_fl")
                    .HasKey(o => new { o.Id });
            }
        }
    }
    