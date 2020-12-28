using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebKompraBinParserContext : WebAvroradataContext
    {
        public DbSet<KompraBinParser> KompraBinParser { get; set; }

    }
}