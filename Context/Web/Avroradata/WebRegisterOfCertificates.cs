using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebRegisterOfCertificates : WebContext
    {
        public DbSet<LocalCertificates> LocalCertificateses { get; set; }
        public DbSet<IndustrialCertificates> IndustrialCertificateses { get; set; }
        public DbSet<ExportCertificates> ExportCertificateses { get; set; }
    }
}