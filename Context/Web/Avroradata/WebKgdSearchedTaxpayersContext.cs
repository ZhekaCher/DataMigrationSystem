using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebKgdSearchedTaxpayersContext : WebAvroradataContext
    {
        public DbSet<KgdSearchedTaxpayers> KgdSearchedTaxpayerses { get; set; }
    }
}