using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.AdataTender
{
    public class WebAdataTenderContext : DbContext
    {
        public WebAdataTenderContext(DbContextOptions<WebAdataTenderContext> options)
            : base(options)
        {
        }

        public WebAdataTenderContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 185.35.223.92; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = adata_tender; Integrated Security=true; Pooling=true;");
        }
    }
}