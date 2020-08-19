using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedOtherParticipantsContext : ParsedContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.ExchangeDocs)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.AffiliatedPersons)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.BoardOfDirectors)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.Shareholders)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.OrgTypes)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.RelationByReports)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.RelationsByContacts)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasMany(x => x.Executors)
                    .WithOne()
                    .HasPrincipalKey(x => x.CodeBin)
                    .HasForeignKey(x => x.CompId);
                modelBuilder.Entity<OtherParticipantDto>()
                    .HasOne(x => x.Acccountant)
                    .WithOne()
                    .HasForeignKey<AcccountantDto>(x => x.CompId)
                    .HasPrincipalKey<OtherParticipantDto>(x => x.CodeBin);
            }
            public DbSet<OtherParticipantDto> OtherParticipantDtos { get; set; }
            public DbSet<ExchangeDocDto> ExchangeDocsDtos { get; set; }
            public DbSet<AffiliatedPersonDto> AffiliatedPersonsDtos { get; set; }
            public DbSet<BoardOfDirectorDto> BoardOfDirectorsDtos { get; set; }
            public DbSet<ShareholderDto> ShareholdersDtos { get; set; }
            public DbSet<OrgTypeDto> OrgTypesDtos { get; set; }
            public DbSet<RelationByReportDto> RelationByReportsDtos { get; set; }
            public DbSet<RelationByContactDto> RelationByContactsDtos { get; set; }
            public DbSet<ExecutorDto> ExecutorsDtos { get; set; }
            public DbSet<AcccountantDto> AcccountantsDtos { get; set; }
    }
}