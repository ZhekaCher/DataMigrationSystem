using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.AdataTender
{
    public class WebAvroradataTenderContext : DbContext
    {
        public WebAvroradataTenderContext(DbContextOptions<WebAvroradataTenderContext> options)
            : base(options)
        {
        }

        public WebAvroradataTenderContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Server = 192.168.1.158; Database = avroradata; Port=5432; User ID = administrator; Password = Z4P6PjEHnJ5nPT; Search Path = adata_tender; Integrated Security=true; Pooling=true;");
        }
    }
}