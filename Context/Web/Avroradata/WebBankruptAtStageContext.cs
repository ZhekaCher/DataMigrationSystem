using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebBankruptAtStageContext : WebContext
    {
        public DbSet<BankruptAtStage> BankruptAtStages { get; set; }
        public DbSet<RegionS> RegionSes { get; set; }
        public DbSet<TypeOfServiceS> TypeOfServiceSes { get; set; }
        public DbSet<BankruptSAddress> BankruptSAddresses { get; set; }
    }
}