using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEgovAutoInfoContext : WebContext
    {
        public DbSet<EgovAutoInfoDto> EgovAutoInfoDtos { get; set; }
    }
}