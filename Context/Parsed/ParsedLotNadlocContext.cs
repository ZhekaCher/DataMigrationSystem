using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedLotNadlocContext : DbContext
    {
        
        public DbSet<NadlocLotsDto> NadlocLotsDtos { get; set; }

        public ParsedLotNadlocContext(DbContextOptions<ParsedLotNadlocContext> options) : base(options)
        {
            
        }

        public ParsedLotNadlocContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'aibek'; Integrated Security=true; Pooling=true;");
        }
    }
}