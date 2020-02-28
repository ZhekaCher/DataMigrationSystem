using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public sealed class EnforcementDebtMigrationService : MigrationService
    {
        private int _counter;
        private readonly object _forLock  = new object();

        public EnforcementDebtMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            _counter = 0;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await using var webEnforcementDebtContext = new WebEnforcementDebtContext();

            var companyDtos = 
                from debtDto in parsedEnforcementDebtContext.EnforcementDebtDtos
                join debtDetail in parsedEnforcementDebtContext.EnforcementDebtDetailDtos 
                    on debtDto.Uid equals debtDetail.Uid into outer
                from debtDetailOuter in outer.DefaultIfEmpty() where debtDto.IinBin % NumOfThreads == numThread
                orderby debtDto.IinBin
                select new EnforcementDebtDto
                {
                    DetailDto = debtDetailOuter,
                    Date = debtDto.Date,
                    Agency = debtDto.Agency,
                    Uid = debtDto.Uid,
                    EssenceRequirements = debtDto.EssenceRequirements,
                    JudicialExecutor = debtDto.JudicialExecutor,
                    IinBin = debtDto.IinBin
                };
            var oldCounter = 0;
            double oldAmount = 0;
            long bin = 0;
            foreach (var companyDto in companyDtos)
            {
                if (bin != companyDto.IinBin)
                {
                    await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"select avroradata.enforcement_debt_history({bin}, {oldCounter}, {oldAmount}::numeric(18,2))");
                    oldCounter = await
                        webEnforcementDebtContext.CompanyEnforcementDebts.CountAsync(x => x.IinBin == companyDto.IinBin);
                    oldAmount = await
                        webEnforcementDebtContext.CompanyEnforcementDebts.Where(x => x.IinBin == companyDto.IinBin).SumAsync(x=>x.Amount);
                    bin = companyDto.IinBin;
                }
                var enforcementDebt = await DtoToCompanyEntity(companyDto, webEnforcementDebtContext);
                await webEnforcementDebtContext.CompanyEnforcementDebts.Upsert(enforcementDebt).On(x => x.Uid).RunAsync();
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
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }

            await Task.WhenAll(tasks);
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await parsedEnforcementDebtContext.Database.ExecuteSqlRawAsync("truncate table avroradata.enforcement_debt cascade;");
        }
        private async Task<CompanyEnforcementDebt> DtoToCompanyEntity(EnforcementDebtDto debtDto, WebEnforcementDebtContext webEnforcementDebtContext)
        {
            var enforcementDebt = new CompanyEnforcementDebt
            {
                Agency =  debtDto.Agency,
                JudicialExecutor = debtDto.JudicialExecutor,
                Date = debtDto.Date,
                EssenceRequirements = debtDto.EssenceRequirements,
                Debtor = debtDto.Debtor,
                Uid = debtDto.Uid,
                IinBin = debtDto.IinBin
            };
            if (debtDto.DetailDto != null)
            {
                enforcementDebt.Amount = debtDto.DetailDto.Amount;
                enforcementDebt.JudicialDoc = debtDto.DetailDto.JudicialDoc;
                enforcementDebt.History = debtDto.DetailDto.History;
                enforcementDebt.Claimer = debtDto.DetailDto.Claimer;
                enforcementDebt.Number = debtDto.DetailDto.Number;
                if (debtDto.DetailDto.Type != null)
                {
                    enforcementDebt.TypeId = (await webEnforcementDebtContext.EnforcementDebtTypes.FirstOrDefaultAsync(x => x.Name == debtDto.DetailDto.Type)).Id;
                }
                else
                {
                    enforcementDebt.TypeId = null;
                }
            }
            return enforcementDebt;
        }
        private async Task MigrateReferences()
        {
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await using var webEnforcementDebtContext = new WebEnforcementDebtContext();
            var debtTypes = parsedEnforcementDebtContext.EnforcementDebtDetailDtos.Select(x => x.Type).Distinct();
            foreach (var distinct in debtTypes)
            {
                await webEnforcementDebtContext.EnforcementDebtTypes.Upsert(new EnforcementDebtType()
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
        }
    }
}