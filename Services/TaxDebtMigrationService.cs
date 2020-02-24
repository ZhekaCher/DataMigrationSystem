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


        private readonly object _forLock;
        private int _counter = 0;
        public TaxDebtMigrationService(int numOfThreads = 1)
        {

            _forLock = new object();
            
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            
            await MigrateReferences();
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
        }

        private async Task Migrate(int numThread)
        {
            await using var parsedTaxDebtContext = new ParsedTaxDebtContext();
            await using var webTaxDebtContext = new WebTaxDebtContext();
            
            var taxDebtDtos = from courtCaseEntityDto in parsedTaxDebtContext.TaxDebts
                join companies in parsedTaxDebtContext.CompanyDtos
                    on courtCaseEntityDto.IinBin equals companies.Bin
                orderby courtCaseEntityDto.IinBin where courtCaseEntityDto.IinBin %NumOfThreads == numThread
                select courtCaseEntityDto;

            foreach (var taxDebtDto in taxDebtDtos)
            {
                var taxDebt = await DtoToEntity(taxDebtDto);
                await webTaxDebtContext.TaxDebts.Upsert(taxDebt).On(x => x.IinBin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
        
        private async Task MigrateReferences()
        {
            await using var parsedTaxDebtContext = new ParsedTaxDebtContext();
            await using var webTaxDebtContext = new WebTaxDebtContext();
            var taxDebtOrgs = parsedTaxDebtContext.TaxDebtOrgs.Select(x=>new 
                {x.CharCode, x.NameKk, x.NameRu}).Distinct();
            foreach (var distinct in taxDebtOrgs)
            {
                var taxDebtOrg = new TaxDebtOrgName
                {
                    CharCode = distinct.CharCode,
                    NameKk = distinct.NameKk,
                    NameRu = distinct.NameRu
                };
                await webTaxDebtContext.TaxDebtOrgNames.Upsert(taxDebtOrg).On(x => x.CharCode).RunAsync();
            }
        
            var taxDebtBccs = parsedTaxDebtContext.TaxDebtBccs.Select(x=> new
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
                await webTaxDebtContext.TaxDebtBccNames.Upsert(taxDebtBcc).On(x => x.Bcc).RunAsync();

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