using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public sealed class EnforcementDebtMigrationService : MigrationService
    {
        private List<EnforcementDebtType> _enforcementDebtTypes;
        private long _counter;
        private readonly object _forLock = new object();
        public EnforcementDebtMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var webEnforcementDebtContext = new WebEnforcementDebtContext();
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            var companiesDto = parsedEnforcementDebtContext.CompanyBinDtos
                .Where(x => x.Code % NumOfThreads == numThread)
                .Include(x => x.EnforcementDebtDtos);
            foreach (var binDto in companiesDto)
            {
                var newList = binDto.EnforcementDebtDtos.Select(x => new EgovEnforcementDebt
                {
                    RelevanceDate = x.RelevanceDate,
                    IinBin = x.IinBin,
                    EnforcementStartDate = x.EnforcementStartDate,
                    RestrictionStartDate = x.RestrictionStartDate,
                    JudicialExecutorKk = x.JudicialExecutorKk,
                    JudicialExecutorRu = x.JudicialExecutorRu,
                    AgencyKk = x.JudicialExecutorKk,
                    AgencyRu = x.AgencyKk,
                    Number = x.Number,
                    Amount = x.Amount,
                    TypeId = _enforcementDebtTypes.FirstOrDefault(f => f.Name == x.TypeRu)?.Id
                }).ToList();
                var oldList = webEnforcementDebtContext.EgovEnforcementDebts.Where(x => x.IinBin == binDto.Code).ToList();
                if ((long) oldList.Sum(x => x.Amount) != (long) newList.Sum(x => x.Amount))
                {
                    await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"insert into avroradata.enforcement_debt_history (biin, count, amount) values ({binDto.Code}, {newList.Count}, {newList.Sum(x => x.Amount)} :: numeric)");
                }
                if (oldList.Count(x => x.RestrictionStartDate != null) != newList.Count(x => x.RestrictionStartDate != null))
                {
                    await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"insert into avroradata.leaving_restriction_history (biin, count) values ({binDto.Code}, {newList.Count(x => x.RestrictionStartDate != null)})");
                }
                await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"delete from avroradata.egov_enforcement_debt where biin = {binDto.Code}");
                await webEnforcementDebtContext.EgovEnforcementDebts.AddRangeAsync(newList);
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }
            await Task.WhenAll(tasks);
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await using var webEnforcementDebtContext = new  WebEnforcementDebtContext();
            await webEnforcementDebtContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");
            await parsedEnforcementDebtContext.Database.ExecuteSqlRawAsync("truncate avroradata.enforcement_debt restart identity;");
        }
        private async Task MigrateReferences()
        {
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await using var webEnforcementDebtContext = new WebEnforcementDebtContext();
            var debtTypes = parsedEnforcementDebtContext.EgovEnforcementDebtDtos.Select(x => new{x.TypeRu, x.TypeKk}).Distinct();
            foreach (var distinct in debtTypes)
            {
                await webEnforcementDebtContext.EnforcementDebtTypes.Upsert(new EnforcementDebtType
                {
                    Name = distinct.TypeRu,
                    NameKk = distinct.TypeRu
                }).On(x => x.Name).RunAsync();
            }
            _enforcementDebtTypes = webEnforcementDebtContext.EnforcementDebtTypes.ToList();
        }
    }
}