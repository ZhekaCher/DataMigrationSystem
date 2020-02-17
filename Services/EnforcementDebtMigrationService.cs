using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class EnforcementDebtMigrationService
    {
        private readonly WebEnforcementDebtContext _enforcementDebtContext;
        private readonly ParsedEnforcementDebtContext _parsedEnforcementDebtContext;
        private readonly IndividualContext _individualContext;
        private readonly CompanyContext _companyContext;

        public EnforcementDebtMigrationService(WebEnforcementDebtContext enforcementDebtContext, ParsedEnforcementDebtContext parsedEnforcementDebtContext, IndividualContext individualContext, CompanyContext companyContext)
        {
            _enforcementDebtContext = enforcementDebtContext;
            _parsedEnforcementDebtContext = parsedEnforcementDebtContext;
            _individualContext = individualContext;
            _companyContext = companyContext;
        }

        public async Task StartMigratingAsync()
        {
            var companyDtos = 
                _parsedEnforcementDebtContext
                    .EnforcementDebtDtos
                    .Where(x =>  
                        _companyContext
                            .Companies
                            .Any(company => x.IinBin == company.Bin))
                    .Include(x=>x.Detail);
            foreach (var companyDto in companyDtos)
            {
                var enforcementDebt = await DtoToCompanyEntity(companyDto);
                var found = await _enforcementDebtContext.EnforcementDebts.FirstOrDefaultAsync(x => x.Uid == enforcementDebt.Uid);
                if (found == null)
                {
                    await _enforcementDebtContext.EnforcementDebts.AddAsync(enforcementDebt);
                }
                else
                {
                    found.Agency = enforcementDebt.Agency;
                    found.Amount = enforcementDebt.Amount;
                    found.JudicialExecutor = enforcementDebt.JudicialExecutor;
                    found.JudicialDoc = enforcementDebt.JudicialDoc;
                    found.History = enforcementDebt.History;
                    found.Debtor = enforcementDebt.Debtor;
                    found.Claimer = enforcementDebt.Claimer;
                    found.Date = enforcementDebt.Date;
                    found.EssenceRequirements = enforcementDebt.EssenceRequirements;
                    found.TypeId = enforcementDebt.TypeId;
                    found.RelevanceDate = enforcementDebt.RelevanceDate;
                    found.Status = enforcementDebt.Status;
                    found.Number = enforcementDebt.Number;
                }
                
                await _enforcementDebtContext.SaveChangesAsync();
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
                Amount = debtDto.Detail.Amount,
                JudicialDoc = debtDto.Detail.JudicialDoc,
                History = debtDto.Detail.History,
                Claimer = debtDto.Detail.Claimer,
                Number = debtDto.Detail.Number,
                Uid = debtDto.Uid,
                IinBin = debtDto.IinBin
            };
            
            if (debtDto.Detail.Type != null)
            {
                enforcementDebt.TypeId = (await _enforcementDebtContext.EnforcementDebtTypes.FirstOrDefaultAsync(x => x.Name == debtDto.Detail.Type)).Id;
            }
            else
            {
                enforcementDebt.TypeId = null;
            }
            return enforcementDebt;
        }
    }
}