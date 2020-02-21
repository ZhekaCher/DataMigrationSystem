using System;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public sealed class EnforcementDebtMigrationService : MigrationService
    {
        private readonly WebEnforcementDebtContext _webEnforcementDebtContext;
        private readonly ParsedEnforcementDebtContext _parsedEnforcementDebtContext;

        public EnforcementDebtMigrationService()
        {
            _webEnforcementDebtContext = new WebEnforcementDebtContext();
            _parsedEnforcementDebtContext = new ParsedEnforcementDebtContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var companyDtos = from debtDto in _parsedEnforcementDebtContext.EnforcementDebtDtos
                join debtDetail in _parsedEnforcementDebtContext.EnforcementDebtDetailDtos
                    on debtDto.Uid equals debtDetail.Uid
                join companies in _parsedEnforcementDebtContext.ParsedCompanies
                    on debtDto.IinBin equals companies.Bin orderby debtDto.IinBin
                select new EnforcementDebtDto
                {
                    DetailDto = debtDetail,
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
                    await _webEnforcementDebtContext.Database.ExecuteSqlRawAsync($"select avroradata.enforcement_debt_history({bin}, {oldCounter}, {oldAmount})");
                    oldCounter = await
                        _webEnforcementDebtContext.CompanyEnforcementDebts.CountAsync(x => x.IinBin == companyDto.IinBin);
                    oldAmount = await
                        _webEnforcementDebtContext.CompanyEnforcementDebts.Where(x => x.IinBin == companyDto.IinBin).SumAsync(x=>x.Amount);
                    bin = companyDto.IinBin;
                    await _webEnforcementDebtContext.SaveChangesAsync();
                }
                var enforcementDebt = await DtoToCompanyEntity(companyDto);
                await _webEnforcementDebtContext.CompanyEnforcementDebts.Upsert(enforcementDebt).On(x => x.Uid).RunAsync();
            }
        }
        private async Task<CompanyEnforcementDebt> DtoToCompanyEntity(EnforcementDebtDto debtDto)
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
                    enforcementDebt.TypeId = (await _webEnforcementDebtContext.EnforcementDebtTypes.FirstOrDefaultAsync(x => x.Name == debtDto.DetailDto.Type)).Id;
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
            var courts = _parsedEnforcementDebtContext.EnforcementDebtDetailDtos.Select(x => x.Type).Distinct();
            foreach (var distinct in courts)
            {
                await _webEnforcementDebtContext.EnforcementDebtTypes.Upsert(new EnforcementDebtType()
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
        }
    }
}