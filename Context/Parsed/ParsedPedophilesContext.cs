using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedPedophilesContext : DbContext
    {
        public ParsedPedophilesContext(DbContextOptions<ParsedPedophilesContext> options) 
            : base(options) {}
        
        public ParsedPedophilesContext(){}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = localhost; Database = test_parsing; Port=5432; User ID = galymzhan; Password = Qwerty123; Search Path = parsing; Integrated Security=true; Pooling=true;");
        }
        public DbSet<PedofilDto> PedofilDtos { get; set; }
    }
}