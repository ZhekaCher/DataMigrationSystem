using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedNewBakruptContext : ParsedContext
    {
        public DbSet<NewBankruptAtStage> NewBankruptAtStages { get; set; }
        public DbSet<NewBankruptCompleted> NewBankruptCompleteds { get; set; }
        public DbSet<NewRehabilityCompleted> NewRehabilityCompleteds { get; set; }
    }
}