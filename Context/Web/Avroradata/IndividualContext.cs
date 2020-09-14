using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class IndividualContext: WebAvroradataContext
    {
        public DbSet<Individual> Individuals { get; set; }
    }
}