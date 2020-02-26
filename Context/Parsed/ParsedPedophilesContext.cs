using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedPedophilesContext : ParsedContext
    {
        
        public DbSet<PedophileDto> PedophileDtos { get; set; }
    }
}