using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedLotNadlocContext : DbContext
    {
        public DbSet<LotNadlocDto> NadlocLotsDtos { get; set; }
        
        public ParsedLotNadlocContext(DbContextOptions<ParsedLotNadlocContext> options) : base(options)
        {
            
        }

        public ParsedLotNadlocContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = '192.168.2.25'; Database = 'adata'; Port='5432'; User ID = 'administrator'; Password = 'Z4P6PjEHnJ5nPT'; Search Path = 'avroradata'; Integrated Security=true; Pooling=true;");
        }

    }
}