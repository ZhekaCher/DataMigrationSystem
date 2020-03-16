using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebTaxpayerRiskDegreeContext:WebContext
    {
        public DbSet<TaxpayerRiskDegree> TaxpayerRiskDegrees { get; set; }
    }
}