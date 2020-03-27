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
 
    }
}