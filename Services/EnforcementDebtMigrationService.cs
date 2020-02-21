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
            // var types = _parsedEnforcementDebtContext.EnforcementDebtDetailDtos.Select(x => x.Type).Distinct();
            // foreach (var type in types)
            // {
            //     var found = await _webEnforcementDebtContext.EnforcementDebtTypes.FirstOrDefaultAsync(x => x.Name == type);
            //     if (found == null)
            //     {
            //         await _webEnforcementDebtContext.EnforcementDebtTypes.AddAsync(new EnforcementDebtType
            //         {
            //             Name = type
            //         });
            //     }
            // }
            // await _webEnforcementDebtContext.SaveChangesAsync();
            
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
            var i = 0;
            long newBin = 0;
            foreach (var companyDto in companyDtos)
            {
                if (newBin != companyDto.IinBin)
                {
                    await _webEnforcementDebtContext.Database.ExecuteSqlRawAsync("");
                    newBin = companyDto.IinBin;
                }
                var enforcementDebt = await DtoToCompanyEntity(companyDto);
                var found = await _webEnforcementDebtContext.CompanyEnforcementDebts.FirstOrDefaultAsync(x => x.Uid == enforcementDebt.Uid);
                if (found == null)
                {
                    await _webEnforcementDebtContext.CompanyEnforcementDebts.AddAsync(enforcementDebt);
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
                await _webEnforcementDebtContext.SaveChangesAsync();
                if (++i== 2000)
                {
                    return;
                }
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
    }
}