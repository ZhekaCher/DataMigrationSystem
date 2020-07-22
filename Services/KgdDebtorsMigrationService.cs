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
            await Migrate();
            await MigrateAgent();
            await MigrateCutomer();
            await using var parsedKgdDebtorsContext = new ParsedKgdDebtorsContext();
            await parsedKgdDebtorsContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.kgd_debtors, avroradata.kgd_debtors_agents,avroradata.kgd_debtors_customers restart identity;");

        }

        private async Task Migrate()
        {
            var parsedKgd = _parsedKgdDebtors.KgdDebtorsDtos;
            await foreach (var kgdDebtorsDto in parsedKgd)
            {
                var kgdDebs = new KgdDebtors
                {
                    IinBiin = kgdDebtorsDto.IinBiin,
                    Fine = kgdDebtorsDto.Fine,
                    Foamy = kgdDebtorsDto.Foamy,
                    Fullname = kgdDebtorsDto.Fullname,
                    MainDebt = kgdDebtorsDto.MainDebt,
                    RelevanceDate = kgdDebtorsDto.RelevanceDate,
                    Rnn = kgdDebtorsDto.Rnn,
                    TotalDebt = kgdDebtorsDto.TotalDebt
                };
                await _webKgdDebtors.KgdDebtors.Upsert(kgdDebs).On(x => x.IinBiin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateAgent()
        {
            var parsedKgdKgdDebtorsAgentsDtos = _parsedKgdDebtors.KgdDebtorsAgentsDtos;
            await foreach (var KgdDebtorsAgentsDto in parsedKgdKgdDebtorsAgentsDtos)
            {
                var t = new KgdDebtorsAgents
                {
                    IinBiin = KgdDebtorsAgentsDto.IinBiin,
                    Fullname = KgdDebtorsAgentsDto.Fullname,
                    RelevanceDate = KgdDebtorsAgentsDto.RelevanceDate,
                    Rnn = KgdDebtorsAgentsDto.Rnn,
                    DebtSum = KgdDebtorsAgentsDto.DebtSum
                };
                await _webKgdDebtors.KgdDebtorsAgents.Upsert(t).On(x => x.IinBiin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateCutomer()
        {
            var kgdDebtorsCustomersDtos = _parsedKgdDebtors.KgdDebtorsCustomersDtos;
            await foreach (var kgdDebtorsCustomersDto in kgdDebtorsCustomersDtos)
            {
                var k = new KgdDebtorsCustomers
                {
                    IinBiin = kgdDebtorsCustomersDto.IinBiin,
                    Fullname = kgdDebtorsCustomersDto.Fullname,
                    DebtSum = kgdDebtorsCustomersDto.DebtSum,
                    RelevanceDate = kgdDebtorsCustomersDto.RelevanceDate
                };
                await _webKgdDebtors.KgdDebtorsCustomers.Upsert(k).On(x => x.IinBiin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}