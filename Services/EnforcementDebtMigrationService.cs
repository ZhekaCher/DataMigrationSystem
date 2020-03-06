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
                parsedEnforcementDebtContext.EnforcementDebtDtos
                    .Where(x => x.IinBin % NumOfThreads == numThread)
                    .OrderBy(x => x.IinBin)
                    .Include(x => x.DetailDto);
            // var oldList = new List<EnforcementDebt>();
            // var newList = new List<EnforcementDebt>();
            // long bin = 0;
            foreach (var companyDto in companyDtos)
            {
                // if (bin != companyDto.IinBin)
                // {
                //     var oldAmount = oldList.Sum(x => x.Amount);
                //     if ((long) newList.Sum(x => x.Amount) != (long) oldAmount || newList.Count != oldList.Count)
                //     {
                //         await webEnforcementDebtContext.EnforcementDebtHistories.AddAsync(new EnforcementDebtHistory
                //         {
                //             Amount = oldAmount,
                //             Count = oldList.Count,
                //             IinBin = bin
                //         });
                //     }
                //     var newDate = newList.Min(x => x.RelevanceDate);
                //     await webEnforcementDebtContext.Database.ExecuteSqlInterpolatedAsync($"update avroradata.enforcement_debt set status = false where relevance_date < {newDate};");
                //     bin = companyDto.IinBin;
                //     oldList = webEnforcementDebtContext.EnforcementDebts.Where(x => x.IinBin == bin && x.Status).ToList();
                //     newList = new List<EnforcementDebt>();
                // }
                var enforcementDebt = await DtoToCompanyEntity(companyDto, webEnforcementDebtContext);
                // await webEnforcementDebtContext.EnforcementDebts.Upsert(enforcementDebt).On(x => x.Uid).RunAsync();
                // newList.Add(enforcementDebt);
                lock (_forLock)
                {
                    Logger.Trace(++_counter);
                }
            }
            // await webEnforcementDebtContext.SaveChangesAsync();

        }
        public override async Task StartMigratingAsync()
        {
            // await MigrateReferences();
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }

            await Task.WhenAll(tasks);
            await using var parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
            await parsedEnforcementDebtContext.Database.ExecuteSqlRawAsync("truncate avroradata.enforcement_debt, avroradata.enforcement_debt_detail restart identity;");
        }
        private async Task<EnforcementDebt> DtoToCompanyEntity(EnforcementDebtDto debtDto, WebEnforcementDebtContext webEnforcementDebtContext)
        {
            var enforcementDebt = new EnforcementDebt
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