using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;


namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedAnnouncementNadlocContext : ParsedContext
    {
        public DbSet<AnnouncementNadlocDto> AnnouncementNadlocDtos { get; set; }

        public DbSet<CompanyBinDto> Companies { get; set; }
        
    }
}