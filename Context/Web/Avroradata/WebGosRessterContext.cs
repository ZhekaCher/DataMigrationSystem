using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebGosRessterContext : WebContext
    {
        public DbSet<GosReester> GosReesters { get; set; }
    }
}