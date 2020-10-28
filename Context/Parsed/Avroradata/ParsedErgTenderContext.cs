﻿using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedErgTenderContext : ParsedAvroradataContext
    {
        public DbSet<ErgTenderDto.ErgTender> ErgTenderes { get; set; }
        public DbSet<ErgTenderDto.ErgTenderPositions> ErgTenderPositionses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErgTenderDto.ErgTender>()
                .HasMany(x => x.ErgTenderPositionses)
                .WithOne()
                .HasForeignKey(x => x.ConcourseId)
                .HasPrincipalKey(x => x.ConcourseId);
        }
    }
}