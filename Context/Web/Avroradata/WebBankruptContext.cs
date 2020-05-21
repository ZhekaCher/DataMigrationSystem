using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebBankruptContext : WebContext
    {
        public DbSet<BankruptAtStage> BankruptAtStages { get; set; }
        public DbSet<RegionS> RegionSes { get; set; }
        public DbSet<TypeOfServiceS> TypeOfServiceSes { get; set; }
        public DbSet<BankruptSAddress> BankruptSAddresses { get; set; }
        public DbSet<BankruptCompleted> BankruptCompleteds { get; set; }
        public DbSet<RegionC> RegionCs { get; set; }
        public DbSet<TypeOfServiceC> TypeOfServiceCs { get; set; }
        public DbSet<BankruptCAddress> BankruptCAddresses { get; set; }
    }
}