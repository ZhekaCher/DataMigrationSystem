﻿using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedRegisterOfCertificatesContext : ParsedAvroradataContext
    {
        public DbSet<LocalCertificatesDto> LocalCertificatesDtos { get; set; }
        public DbSet<IndustrialCertificatesDto> IndustrialCertificatesDtos { get; set; }
        public DbSet<ExportCertificatesDto> ExportCertificatesDtos { get; set; }
       
        public DbSet<CompanyBinDto> CompanyBinDtos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyBinDto>()
                .HasMany(x => x.LocalCertificatesDto)
                .WithOne()
                .HasPrincipalKey(x => x.Code)
                .HasForeignKey(x => x.ManufacturerIinBiin);
            modelBuilder.Entity<CompanyBinDto>()
                .HasMany(x => x.ExportCertificatesDtos)
                .WithOne()
                .HasPrincipalKey(x => x.Code)
                .HasForeignKey(x => x.ManufacturerIinBiin);
            modelBuilder.Entity<CompanyBinDto>()
                .HasMany(x => x.IndustrialCertificatesDtos)
                .WithOne()
                .HasPrincipalKey(x => x.Code)
                .HasForeignKey(x => x.IinBiin);
        }
        
    }
}