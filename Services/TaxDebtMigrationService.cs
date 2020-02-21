using System;
using System.Collections.Generic;
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
    public class TaxDebtMigrationService : MigrationService
    {
        private readonly ParsedTaxDebtContext _parsedTaxDebtContext;
        private readonly WebTaxDebtContext _webTaxDebtContext;

        public TaxDebtMigrationService()
        {
            _parsedTaxDebtContext = new ParsedTaxDebtContext();
            _webTaxDebtContext = new WebTaxDebtContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var taxDebtDtos = _parsedTaxDebtContext.TaxDebts;

            foreach (var taxDebtDto in taxDebtDtos)
            {
                var taxDebt = await DtoToEntity(taxDebtDto);
                await _webTaxDebtContext.TaxDebts.Upsert(taxDebt).On(x => x.IinBin).RunAsync();
            }
        }
        
        private async Task MigrateReferences()
        {
            var taxDebtOrgs = _parsedTaxDebtContext.TaxDebtOrgs.Select(x=>new {x.CharCode, x.NameKk, x.NameRu}).Distinct();
            foreach (var distinct in taxDebtOrgs)
            {
                var taxDebtOrg = new TaxDebtOrgName
                {
                    CharCode = distinct.CharCode,
                    NameKk = distinct.NameKk,
                    NameRu = distinct.NameRu
                };
                await _webTaxDebtContext.TaxDebtOrgNames.Upsert(taxDebtOrg).On(x => x.CharCode).RunAsync();
            }
        
            var taxDebtBccs = _parsedTaxDebtContext.TaxDebtBccs.Select(x=> new
            {
                x.Bcc, x.NameKk, x.NameRu
            }).Distinct();
            foreach (var distinct in taxDebtBccs)
            {
                var taxDebtBcc = new TaxDebtBccName
                {
                    Bcc = distinct.Bcc,
                    NameKk = distinct.NameKk,
                    NameRu = distinct.NameRu
                };
                await _webTaxDebtContext.TaxDebtBccNames.Upsert(taxDebtBcc).On(x => x.Bcc).RunAsync();

            }
        }
        
        private async Task<TaxDebt> DtoToEntity(TaxDebtDto debtDto)
        {
            return await Task.Run(() =>
            {
                // var taxOrgs = new List<TaxDebtOrg>();
                // foreach (var taxOrgInfo in debtDto.TaxDebtOrgs)
                // {
                //
                //     var taxPayers = new List<TaxDebtPayer>();
                //     foreach (var taxPayerInfo in taxOrgInfo.TaxDebtPayers)
                //     {
                //         var taxBccs = new List<TaxDebtBcc>();
                //         foreach (var bccArrearsInfo in taxPayerInfo.TaxDebtBccs)
                //         {
                //         
                //             taxBccs.Add(new TaxDebtBcc
                //             {
                //                 Bcc = bccArrearsInfo.Bcc,
                //                 Tax = bccArrearsInfo.Tax,
                //                 Total = bccArrearsInfo.Total,
                //                 Poena = bccArrearsInfo.Poena,
                //                 Fine = bccArrearsInfo.Fine,
                //                 CharCode = bccArrearsInfo.CharCode,
                //                 IinBin = bccArrearsInfo.IinBin,
                //             });
                //         }
                //         taxPayers.Add(new TaxDebtPayer
                //         {
                //             IinBin = taxPayerInfo.IinBin,
                //             CharCode = taxPayerInfo.CharCode,
                //             HeadIinBin = taxPayerInfo.HeadIinBin,
                //             Total = taxPayerInfo.Total,
                //             TaxDebtBccs = taxBccs
                //         });
                //     }
                //     taxOrgs.Add(new TaxDebtOrg
                //     {
                //         CharCode = taxOrgInfo.CharCode,
                //         Total = taxOrgInfo.Total,
                //         TotalTax = taxOrgInfo.TotalTax,
                //         PensionContribution = taxOrgInfo.PensionContribution,
                //         SocialContribution = taxOrgInfo.SocialContribution,
                //         SocialHealthInsurance = taxOrgInfo.SocialHealthInsurance,
                //         IinBin = taxOrgInfo.IinBin,
                //         TaxDebtPayers = taxPayers
                //     });
                // }
                var taxDebt = new TaxDebt
                {
                    Total = debtDto.Total,
                    PensionContribution = debtDto.PensionContribution,
                    SocialContribution = debtDto.SocialContribution,
                    SocialHealthInsurance = debtDto.SocialHealthInsurance,
                    IinBin = debtDto.IinBin,
                    // TaxDebtOrgs = taxOrgs
                };
                return taxDebt;
            });
        }
    }
}