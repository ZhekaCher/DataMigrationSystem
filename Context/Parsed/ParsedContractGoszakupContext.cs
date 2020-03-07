using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:52:27
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'contract_goszakup'
    /// </summary>
    public class ParsedContractGoszakupContext : ParsedContext
    {
        public DbSet<ContractGoszakupDto> ContractGoszakupDtos { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
            // optionsBuilder.UseNpgsql(
                // "Server = '192.168.2.37'; Database = 'development'; Port='5432'; User ID = 'administrator'; Password = 'administrator'; Search Path = 'evgeniy'; Integrated Security=true; Pooling=true;");
        // }
    }
}