using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebBankruptCompletedContext : WebContext
    {
        public DbSet<BankruptCompleted> BankruptCompleteds { get; set; }
        public DbSet<RegionC> RegionCs { get; set; }
        public DbSet<TypeOfServiceC> TypeOfServiceCs { get; set; }
        public DbSet<BankruptCAddress> BankruptCAddresses { get; set; }
    }
}