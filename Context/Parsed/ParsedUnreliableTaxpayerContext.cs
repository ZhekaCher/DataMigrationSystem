using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedUnreliableTaxpayerContext: DbContext
    {
        public ParsedUnreliableTaxpayerContext(DbContextOptions<ParsedUnreliableTaxpayerContext> options)
            : base(options)
        {
            
        }

        public ParsedUnreliableTaxpayerContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<UnreliableTaxpayerDto> UnreliableTaxpayerDtos { get; set; }    
    }
}