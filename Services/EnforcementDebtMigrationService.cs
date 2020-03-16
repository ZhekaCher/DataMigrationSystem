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
            var egovEnforcementDebts = from egovEnforcementDebtDto in parsedEnforcementDebtContext.EgovEnforcementDebtDtos
                where egovEnforcementDebtDto.IinBin%NumOfThreads==numThread
                orderby egovEnforcementDebtDto.IinBin
                select new EgovEnforcementDebt
                {
                    RelevanceDate = egovEnforcementDebtDto.RelevanceDate,
                    IinBin = egovEnforcementDebtDto.IinBin,
                    EnforcementStartDate = egovEnforcementDebtDto.EnforcementStartDate,
                    RestrictionStartDate = egovEnforcementDebtDto.RestrictionStartDate,
                    JudicialExecutorKk = egovEnforcementDebtDto.JudicialExecutorKk,
                    JudicialExecutorRu = egovEnforcementDebtDto.JudicialExecutorRu,
                    AgencyKk = egovEnforcementDebtDto.JudicialExecutorKk,
                    AgencyRu = egovEnforcementDebtDto.AgencyKk,
                    Number = egovEnforcementDebtDto.Number,
                    Amount = egovEnforcementDebtDto.Amount,
                    TypeId = _enforcementDebtTypes.FirstOrDefault(x=>x.Name == egovEnforcementDebtDto.TypeRu)?.Id
                };

            long bin = 0;
            var oldList = new List<double>();
            var newList = new List<double>();
            foreach (var companyDto in egovEnforcementDebts)
            {
                if (bin != companyDto.IinBin)
                {
                    var oldAmount = oldList.Sum();
                    if ((long) newList.Sum() != (long) oldAmount || newList.Count != oldList.Count)
                    {
                        await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"insert into avroradata.enforcement_debt_history (biin, count, amount) values ({bin}, {oldList.Count}, {oldAmount} :: numeric)");
                    }                    
                    bin = companyDto.IinBin;
                    oldList = webEnforcementDebtContext.EgovEnforcementDebts.Where(x => x.IinBin == bin)
                        .Select(x => x.Amount).ToList();
                    await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"delete from avroradata.egov_enforcement_debt where biin = {bin}");
                    newList = new List<double>();
                }
                await webEnforcementDebtContext.EgovEnforcementDebts.AddAsync(companyDto);
                await webEnforcementDebtContext.SaveChangesAsync();
                newList.Add(companyDto.Amount);
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