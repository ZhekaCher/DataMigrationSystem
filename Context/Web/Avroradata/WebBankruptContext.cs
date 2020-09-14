using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebNewBankruptContext : WebAvroradataContext
    {
        public DbSet<NewBankruptAtStage> NewBankruptAtStages { get; set; }
        public DbSet<NewRehabilityCompleted> NewRehabilityCompleteds { get; set; }
        public DbSet<NewBankruptCompleted> NewBankruptCompleteds { get; set; }
    }
}