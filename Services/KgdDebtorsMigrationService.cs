using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class KgdDebtorsMigrationService : MigrationService
    {
        private readonly WebKgdDebtorsContext _webKgdDebtors;
        private readonly ParsedKgdDebtorsContext _parsedKgdDebtors;
        private readonly object _forLock;
        private int _counter;
        private readonly Dictionary<string, long> _dictionary = new Dictionary<string, long>();


        public KgdDebtorsMigrationService(int numOfThreads = 30)
        {
            _webKgdDebtors = new WebKgdDebtorsContext();
            _parsedKgdDebtors = new ParsedKgdDebtorsContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            await Migrate();
            await MigrateAgent();
            await MigrateCutomer();
            await using var parsedKgdDebtorsContext = new ParsedKgdDebtorsContext();
            await parsedKgdDebtorsContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.kgd_debtors, avroradata.kgd_debtors_agents,avroradata.kgd_debtors_customers restart identity;");

        }

        private async Task MigrateReferences()
        {
           
            var categories =await _parsedKgdDebtors.KgdDebtorsDtos
                .Select(x => new KgdAllDebtorsCategory
                {
                    Category = x.Category
                }).Distinct().ToListAsync();
            categories.AddRange(await _parsedKgdDebtors.KgdDebtorsAgentsDtos
                .Select(x => new KgdAllDebtorsCategory
                {
                    Category = x.Category
                }).Distinct().ToListAsync());
            categories.AddRange(await _parsedKgdDebtors.KgdDebtorsCustomersDtos
                .Select(x => new KgdAllDebtorsCategory
                {
                    Category = x.Category
                }).Distinct().ToListAsync());
            
            await _webKgdDebtors.KgdAllDebtorsCategory.UpsertRange(categories).On(x => x.Category).RunAsync();
            foreach (var category in _webKgdDebtors.KgdAllDebtorsCategory)
            {
                _dictionary[category.Category] = category.Id;
                Console.WriteLine();
            }
        }

        private async Task Migrate()
        {
            await foreach (var kgdDebtorsDto in _parsedKgdDebtors.KgdDebtorsDtos)
            {
                var kgdDebs = new KgdDebtors
                {
                    IinBiin = kgdDebtorsDto.IinBiin,
                    Fine = kgdDebtorsDto.Fine,
                    Foamy = kgdDebtorsDto.Foamy,
                    Fullname = kgdDebtorsDto.Fullname,
                    MainDebt = kgdDebtorsDto.MainDebt,
                    RelevanceDate = kgdDebtorsDto.RelevanceDate,
                    Code = kgdDebtorsDto.Code,
                    TotalDebt = kgdDebtorsDto.TotalDebt,
                    CategoryDate = kgdDebtorsDto.CategoryDate,
                    CategoryId = _dictionary[kgdDebtorsDto.Category],
                    ParseDate = kgdDebtorsDto.ParseDate
                };
                await _webKgdDebtors.KgdDebtors.Upsert(kgdDebs).On(x => new {x.IinBiin,x.CategoryId,x.Code}).RunAsync();
                await _webKgdDebtors.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateAgent()
        {
            await foreach (var kgdDebtorsAgentsDto in _parsedKgdDebtors.KgdDebtorsAgentsDtos)
            {
                var kgdDebtorsAgents = new KgdDebtorsAgents
                {
                    IinBiin = kgdDebtorsAgentsDto.IinBiin,
                    Fullname = kgdDebtorsAgentsDto.Fullname,
                    RelevanceDate = kgdDebtorsAgentsDto.RelevanceDate,
                    DebtSum = kgdDebtorsAgentsDto.DebtSum,
                    CategoryDate = kgdDebtorsAgentsDto.CategoryDate,
                    CategoryId = _dictionary[kgdDebtorsAgentsDto.Category],
                    ParseDate = kgdDebtorsAgentsDto.ParseDate
                };
                await _webKgdDebtors.KgdDebtorsAgents.Upsert(kgdDebtorsAgents).On(x => new{x.IinBiin,x.CategoryId}).RunAsync();
                await _webKgdDebtors.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateCutomer()
        {
            await foreach (var kgdDebtorsCustomersDto in _parsedKgdDebtors.KgdDebtorsCustomersDtos)
            {
                var kgdDebtorsCustomers = new KgdDebtorsCustomers
                {
                    IinBiin = kgdDebtorsCustomersDto.IinBiin,
                    Fullname = kgdDebtorsCustomersDto.Fullname,
                    DebtSum = kgdDebtorsCustomersDto.DebtSum,
                    RelevanceDate = kgdDebtorsCustomersDto.RelevanceDate,
                    CategoryDate = kgdDebtorsCustomersDto.CategoryDate,
                    CategoryId = _dictionary[kgdDebtorsCustomersDto.Category],
                    ParseDate = kgdDebtorsCustomersDto.ParseDate
                };
                await _webKgdDebtors.KgdDebtorsCustomers.Upsert(kgdDebtorsCustomers).On(x => new{x.IinBiin,x.CategoryId}).RunAsync();
                await _webKgdDebtors.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}