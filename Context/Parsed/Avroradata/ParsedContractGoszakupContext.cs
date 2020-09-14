using DataMigrationSystem.Models.Parsed.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 17:52:27
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'contract_goszakup'
    /// </summary>
    public class ParsedContractGoszakupContext : ParsedAvroradataContext
    {
        public DbSet<ContractGoszakupDto> ContractGoszakupDtos { get; set; }
 
    }
}