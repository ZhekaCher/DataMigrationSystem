using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebEgovAutoInfoContext : WebAvroradataContext
    {
        public DbSet<EgovAutoInfoDto> EgovAutoInfoDtos { get; set; }
    }
}