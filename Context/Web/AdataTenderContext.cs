using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web
{
    public class AdataTenderContext: DbContext
    {

        public DbSet<AdataAnnouncement> AdataAnnouncements { get; set; }    
        public DbSet<AnnouncementContact> AnnouncementContacts { get; set; }    
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
        
        public AdataTenderContext(DbContextOptions<WebContext> options)
            : base(options)
        {
        }

        public AdataTenderContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.25; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = adata_tender; Integrated Security=true; Pooling=true;");
        }
    }
}