using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class IndividualContext: WebContext
    {
        public DbSet<Individual> Individuals { get; set; }
    }
}