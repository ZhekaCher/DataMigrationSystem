using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 16:02:50
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей 'unscrupulous_goszakup'
    /// </summary>

    public class WebUnscrupulousGoszakupContext : WebContext
    {
        public DbSet<UnscrupulousGoszakup> UnscrupulousGoszakup { get; set; }
    }
}