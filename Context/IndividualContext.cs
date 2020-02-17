using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{
    public class IndividualContext: DbContext
    {
        public IndividualContext(DbContextOptions<IndividualContext> options)
            : base(options)
        {
            
        }
        public DbSet<Individual> Individuals { get; set; }
    }
    public class CompanyContext: DbContext
    {
        public CompanyContext(DbContextOptions<CompanyContext> options)
            : base(options)
        {
            
        }

        public DbSet<Company> Companies { get; set; }
       
    }
}