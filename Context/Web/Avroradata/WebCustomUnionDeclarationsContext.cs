using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCustomUnionDeclarationsContext : WebAvroradataContext
    {
        public DbSet<CustomUnionDeclarations> CustomUnionDeclarationses { get; set; }
        public DbSet<CustomUnionDeclarationsAd> CustomUnionDeclarationsAds { get; set; }
    }
}