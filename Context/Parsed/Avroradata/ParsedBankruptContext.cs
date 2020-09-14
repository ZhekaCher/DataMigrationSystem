using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    public class ParsedNewBakruptContext : ParsedAvroradataContext
    {
        public DbSet<NewBankruptAtStage> NewBankruptAtStages { get; set; }
        public DbSet<NewBankruptCompleted> NewBankruptCompleteds { get; set; }
        public DbSet<NewRehabilityCompleted> NewRehabilityCompleteds { get; set; }
    }
}