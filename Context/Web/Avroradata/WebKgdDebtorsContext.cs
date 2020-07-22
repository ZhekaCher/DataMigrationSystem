using System;
using System.Threading.Tasks;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebKgdDebtorsContext : WebContext
    {
        public DbSet<KgdDebtors> KgdDebtors { get; set; }
        public DbSet<KgdDebtorsAgents> KgdDebtorsAgents { get; set; }
        public DbSet<KgdDebtorsCustomers> KgdDebtorsCustomers { get; set; }
    }
}