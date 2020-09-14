using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 29.02.2020 15:28:43
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    /// <code>
    /// 
    /// </code>


    public class ParsedDirectorGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<DirectorGoszakupDto> DirectorGoszakupDtos { get; set; } 
    }
}