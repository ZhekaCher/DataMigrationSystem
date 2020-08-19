using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebOtherParticipantsContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = '192.168.1.158'; Database = 'avroradata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'other_participants'; Integrated Security=true; Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.ExchangeDocs)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.AffiliatedPersons)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.BoardOfDirectors)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.Shareholders)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.OrgTypes)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.RelationByReports)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.RelationsByContacts)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasMany(x => x.Executors)
                .WithOne()
                .HasPrincipalKey(x => x.CodeBin)
                .HasForeignKey(x => x.CompId);
            modelBuilder.Entity<OtherParticipant>()
                .HasOne(x => x.Acccountant)
                .WithOne()
                .HasForeignKey<Acccountant>(x => x.CompId)
                .HasPrincipalKey<OtherParticipant>(x => x.CodeBin);
        }

        public DbSet<OtherParticipant> MainInfos { get; set; }
        public DbSet<ExchangeDoc> ExchangeDocs { get; set; }
        public DbSet<AffiliatedPerson> AffiliatedPersons { get; set; }
        public DbSet<BoardOfDirector> BoardOfDirectors { get; set; }
        public DbSet<Shareholder> Shareholders { get; set; }
        public DbSet<OrgType> OrgTypes { get; set; }
        public DbSet<RelationByReport> RelationByReports { get; set; }
        public DbSet<RelationByContact> RelationByContacts { get; set; }
        public DbSet<Executor> Executors { get; set; }
        public DbSet<Acccountant> Acccountants { get; set; }
    }
}