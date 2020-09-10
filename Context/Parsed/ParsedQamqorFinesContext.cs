using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedQamqorFinesContext : ParsedContext
    {
        public DbSet<QamqorFinesDto> QamqorFinesDtos { get; set; }
    }
}