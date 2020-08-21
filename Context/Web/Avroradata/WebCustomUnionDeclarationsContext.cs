using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebCustomUnionDeclarationsContext : WebContext
    {
        public DbSet<CustomUnionDeclarations> CustomUnionDeclarationses { get; set; }
        public DbSet<CustomUnionDeclarationsAd> CustomUnionDeclarationsAds { get; set; }
    }
}