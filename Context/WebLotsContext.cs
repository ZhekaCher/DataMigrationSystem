using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 19:20:01
    /// @version 1.0
    /// <summary>
    /// Контекст для работы с таблицей ''
    /// </summary>


    public class WebLotsContext : DbContext
    {
        public WebLotsContext(DbContextOptions<WebLotsContext> options)
            : base(options)
        {
            
        }

        public WebLotsContext()
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server = ''; Database = ''; Port=''; User ID = ''; Password = ''; Search Path = ''; Integrated Security=true; Pooling=true;");
        }
    }
}