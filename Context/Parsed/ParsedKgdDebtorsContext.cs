using System;
using System.Threading.Tasks;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Parsed
{
    public class ParsedKgdDebtorsContext : ParsedContext
    {
        public DbSet<KgdDebtorsDto> KgdDebtorsDtos { get; set; }
        public DbSet<KgdDebtorsAgentsDto> KgdDebtorsAgentsDtos { get; set; }
        public DbSet<KgdDebtorsCustomersDto> KgdDebtorsCustomersDtos { get; set; }
    }
}