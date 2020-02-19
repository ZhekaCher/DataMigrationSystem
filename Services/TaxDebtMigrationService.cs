using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TaxDebtMigrationService : MigrationService
    {
        private readonly ParsedTaxDebtContext _parsedTaxDebtContext;
        private readonly WebTaxDebtContext _webTaxDebtContext;

        public TaxDebtMigrationService(ParsedTaxDebtContext parsedTaxDebtContext, WebTaxDebtContext webTaxDebtContext)
        {
            _parsedTaxDebtContext = parsedTaxDebtContext;
            _webTaxDebtContext = webTaxDebtContext;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var taxDebtDtos = _parsedTaxDebtContext.TaxDebts.Include(x=>x.TaxDebtOrgs).ThenInclude(x=>x.TaxDebtPayers).ThenInclude(x=>x.TaxDebtBccs);
            foreach (var taxDebtDto in taxDebtDtos)
            {
                var taxDebt = await DtoToEntity(taxDebtDto);
                var found = await _webTaxDebtContext.TaxDebts
                    .Include(x=>x.TaxDebtOrgs)
                    .ThenInclude(x=>x.TaxDebtPayers)
                    .ThenInclude(x=>x.TaxDebtBccs)
                    .FirstOrDefaultAsync(x => x.IinBin == taxDebtDto.IinBin);
                if (found == null)
                {
                    await _webTaxDebtContext.TaxDebts.AddAsync(taxDebt);
                }
                else
                {
                    found.Total = taxDebt.Total;
                    found.PensionContribution = taxDebt.PensionContribution;
                    found.SocialContribution = taxDebt.SocialContribution;
                    found.SocialHealthInsurance = taxDebt.SocialHealthInsurance;
                    found.RelevanceDate = taxDebt.RelevanceDate;
                    _webTaxDebtContext.TaxDebtOrgs.RemoveRange(found.TaxDebtOrgs);
                    await _webTaxDebtContext.SaveChangesAsync();
                    await _webTaxDebtContext.TaxDebtOrgs.AddRangeAsync(taxDebt.TaxDebtOrgs);
                }
                await _webTaxDebtContext.SaveChangesAsync();
            }
        }
        
        private async Task MigrateReferences()
        {
            var taxDebtOrgs = _parsedTaxDebtContext.TaxDebtOrgNames;
            foreach (var distinct in taxDebtOrgs)
            {
                var found = await _webTaxDebtContext.TaxDebtOrgNames.FirstOrDefaultAsync(x =>
                    x.CharCode == distinct.CharCode);
                if (found == null)
                {
                    await _webTaxDebtContext.TaxDebtOrgNames.AddAsync(new TaxDebtOrgName
                    {
                        CharCode = distinct.CharCode,
                        NameKk = distinct.NameKk,
                        NameRu = distinct.NameRu
                    });
                }
            }

            await _webTaxDebtContext.SaveChangesAsync();
            var taxDebtBccs = _parsedTaxDebtContext.TaxDebtBccNames;
            foreach (var distinct in taxDebtBccs)
            {
                var found = await _webTaxDebtContext.TaxDebtBccNames.FirstOrDefaultAsync(x =>
                    x.Bcc == distinct.Bcc);
                if (found == null)
                {
                    await _webTaxDebtContext.TaxDebtBccNames.AddAsync(new TaxDebtBccName
                    {
                        Bcc = distinct.Bcc,
                        NameKk = distinct.NameKk,
                        NameRu = distinct.NameRu
                    });
                }
            }
            await _webTaxDebtContext.SaveChangesAsync();
        }
        
        private async Task<TaxDebt> DtoToEntity(TaxDebtDto debtDto)
        {
            return await Task.Run(() =>
            {
                var taxOrgs = new List<TaxDebtOrg>();
                foreach (var taxOrgInfo in debtDto.TaxDebtOrgs)
                {
                
                    var taxPayers = new List<TaxDebtPayer>();
                    foreach (var taxPayerInfo in taxOrgInfo.TaxDebtPayers)
                    {
                        var taxBccs = new List<TaxDebtBcc>();
                        foreach (var bccArrearsInfo in taxPayerInfo.TaxDebtBccs)
                        {
                        
                            taxBccs.Add(new TaxDebtBcc
                            {
                                Bcc = bccArrearsInfo.Bcc,
                                Tax = bccArrearsInfo.Tax,
                                Total = bccArrearsInfo.Total,
                                Poena = bccArrearsInfo.Poena,
                                Fine = bccArrearsInfo.Fine,
                                CharCode = bccArrearsInfo.CharCode,
                                IinBin = bccArrearsInfo.IinBin,
                            });
                        }
                        taxPayers.Add(new TaxDebtPayer
                        {
                            IinBin = taxPayerInfo.IinBin,
                            CharCode = taxPayerInfo.CharCode,
                            HeadIinBin = taxPayerInfo.HeadIinBin,
                            Total = taxPayerInfo.Total,
                            TaxDebtBccs = taxBccs
                        });
                    }
                    taxOrgs.Add(new TaxDebtOrg
                    {
                        CharCode = taxOrgInfo.CharCode,
                        Total = taxOrgInfo.Total,
                        TotalTax = taxOrgInfo.TotalTax,
                        PensionContribution = taxOrgInfo.PensionContribution,
                        SocialContribution = taxOrgInfo.SocialContribution,
                        SocialHealthInsurance = taxOrgInfo.SocialHealthInsurance,
                        IinBin = taxOrgInfo.IinBin,
                        TaxDebtPayers = taxPayers
                    });
                }
                var taxDebt = new TaxDebt
                {
                    Total = debtDto.Total,
                    PensionContribution = debtDto.PensionContribution,
                    SocialContribution = debtDto.SocialContribution,
                    SocialHealthInsurance = debtDto.SocialHealthInsurance,
                    IinBin = debtDto.IinBin,
                    TaxDebtOrgs = taxOrgs
                };
                return taxDebt;
            });
        }
    }
}