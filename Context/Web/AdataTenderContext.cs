using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web
{
    public class AdataTenderContext: DbContext
    {

        public DbSet<AdataAnnouncement> AdataAnnouncements { get; set; }    
        public DbSet<AnnouncementDocumentation> AnnouncementDocumentations { get; set; }    
        public DbSet<AdataLot> AdataLots { get; set; }    
        public DbSet<LotDocumentation> LotDocumentations { get; set; }    
        public DbSet<Status> Statuses { get; set; }    
        public DbSet<CombinedStatus> CombinedStatuses { get; set; }    
        public DbSet<Method> Methods { get; set; }    
        public DbSet<CombinedMethod> CombinedMethods { get; set; }    
        public DbSet<Measure> Measures { get; set; }    
        public DbSet<TruCode> TruCodes { get; set; }    
        public DbSet<DocumentationType> DocumentationTypes { get; set; }  
        public DbSet<TenderPriority> TenderPriorities { get; set; }    
        public DbSet<PaymentCondition> PaymentConditions { get; set; }    

        public AdataTenderContext(DbContextOptions<WebContext> options)
            : base(options)
        {
        }

        public AdataTenderContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = adata_tender; Integrated Security=true; Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdataAnnouncement>()
                .HasMany(x => x.Lots)
                .WithOne()
                .HasForeignKey(x => x.AnnouncementId)
                .HasPrincipalKey(x => x.Id);
            modelBuilder.Entity<AdataAnnouncement>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.AnnouncementId)
                .HasPrincipalKey(x => x.Id);
            modelBuilder.Entity<AdataLot>()
                .HasMany(x => x.Documentations)
                .WithOne()
                .HasForeignKey(x => x.LotId)
                .HasPrincipalKey(x => x.Id);
            modelBuilder.Entity<AdataLot>()
                .HasOne(x => x.PaymentCondition)
                .WithOne()
                .HasForeignKey<PaymentCondition>(x => x.LotId)
                .HasPrincipalKey<AdataLot>(x => x.Id);
        }
    }
}