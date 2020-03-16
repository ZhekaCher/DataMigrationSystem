using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTaxDetailsContext:WebContext
    {
        public DbSet<TaxDetails> TaxDetailses { get; set; }
    }
}