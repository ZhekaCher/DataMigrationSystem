using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TaxDebtMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter = 0;
        public TaxDebtMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
            _forLock = new object();
            
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            
            await MigrateReferences();
            
            await Migrate();
            
            await using var parsedTaxDebtContext = new ParsedTaxDebtContext();
            await using var webTaxDebtContext = new WebTaxDebtContext();
            await parsedTaxDebtContext.Database.ExecuteSqlRawAsync("truncate avroradata.tax_debt restart identity cascade;");
            await webTaxDebtContext.Database.ExecuteSqlRawAsync("call avroradata.unreliable_companies_updater();");

        }

        private async Task Insert(TaxDebt taxDebt)
        {
            await using var webTaxDebtContext = new WebTaxDebtContext();
            await webTaxDebtContext.TaxDebts.Upsert(taxDebt).On(x => x.IinBin).RunAsync();
            foreach (var taxDebtOrg in taxDebt.TaxDebtOrgs)
            {
                await webTaxDebtContext.TaxDebtOrgs.Upsert(taxDebtOrg).On(x => new {x.IinBin, x.CharCode}).RunAsync();
                foreach (var taxDebtPayer in taxDebtOrg.TaxDebtPayers)
                {
                    await webTaxDebtContext.TaxDebtPayers.Upsert(taxDebtPayer).On(x => new {x.IinBin, x.CharCode}).RunAsync();
                    foreach (var taxDebtBcc in taxDebtPayer.TaxDebtBccs)
                    {
                        await webTaxDebtContext.TaxDebtBccs.Upsert(taxDebtBcc).On(x => new {x.IinBin, x.CharCode, x.Bcc}).RunAsync();
                    }
                }
            }
            lock (_forLock)
            {
                Logger.Trace(_counter++);
            }
        }
        private async Task Migrate()
        {
            await using var parsedTaxDebtContext = new ParsedTaxDebtContext();
            parsedTaxDebtContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var taxDebts = parsedTaxDebtContext.TaxDebts.AsNoTracking()
                .Include(x=>x.TaxDebtOrgs)
                .ThenInclude(x=>x.TaxDebtPayers)
                .ThenInclude(x=>x.TaxDebtBccs)
                .Select(taxDebt => new TaxDebt
                {
                    Total = taxDebt.Total,
                    PensionContribution = taxDebt.PensionContribution,
                    SocialContribution = taxDebt.SocialContribution,
                    SocialHealthInsurance = taxDebt.SocialHealthInsurance,
                    IinBin = taxDebt.IinBin,
                    RelevanceDate = taxDebt.RelevanceDate,
                    TaxDebtOrgs = taxDebt.TaxDebtOrgs.Select(taxDebtOrg=> new TaxDebtOrg
                    {
                        Total = taxDebtOrg.Total,
                        TotalTax = taxDebtOrg.TotalTax,
                        PensionContribution = taxDebtOrg.PensionContribution,
                        SocialContribution = taxDebtOrg.SocialContribution,
                        SocialHealthInsurance = taxDebtOrg.SocialHealthInsurance,
                        IinBin = taxDebtOrg.IinBin,
                        CharCode = taxDebtOrg.CharCode,
                        TaxDebtPayers = taxDebtOrg.TaxDebtPayers.Select(taxDebtPayer => new TaxDebtPayer
                        {
                            IinBin = taxDebtPayer.IinBin,
                            HeadIinBin = taxDebtPayer.HeadIinBin,
                            Total = taxDebtPayer.Total,
                            CharCode = taxDebtPayer.CharCode,
                            TaxDebtBccs = taxDebtPayer.TaxDebtBccs.Select(taxDebtBcc => new TaxDebtBcc
                            {
                                IinBin = taxDebtBcc.IinBin,
                                Fine = taxDebtBcc.Fine,
                                Total = taxDebtBcc.Total,
                                CharCode = taxDebtBcc.CharCode,
                                Tax = taxDebtBcc.Tax,
                                Poena = taxDebtBcc.Poena,
                                Bcc = taxDebtBcc.Bcc
                            })
                        })
                    })
                });
            
            var tasks = new List<Task>();
            foreach (var taxDebt in taxDebts)
            {
                tasks.Add(Insert(taxDebt));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            
            await Task.WhenAll(tasks);
        }
        
        private async Task MigrateReferences()
        {
            await using var parsedTaxDebtContext = new ParsedTaxDebtContext();
            await using var webTaxDebtContext = new WebTaxDebtContext();
            var taxDebtOrgs = parsedTaxDebtContext.TaxDebtOrgs
                .Select(distinct=> new TaxDebtOrgName 
                {
                    CharCode = distinct.CharCode,
                    NameKk = distinct.NameKk,
                    NameRu = distinct.NameRu
                })
                .Distinct();
            await webTaxDebtContext.TaxDebtOrgNames.UpsertRange(taxDebtOrgs).On(x => x.CharCode).RunAsync();

            var taxDebtBccs = parsedTaxDebtContext.TaxDebtBccs
                .Select(x=> new TaxDebtBccName
                {
                    Bcc = x.Bcc, 
                    NameKk = x.NameKk, 
                    NameRu = x.NameRu
                }).Distinct();
            await webTaxDebtContext.TaxDebtBccNames.UpsertRange(taxDebtBccs).On(x => x.Bcc).RunAsync();
        }
        
        // private async Task<TaxDebt> DtoToEntity(TaxDebtDto debtDto)
        // {
        //     return await Task.Run(() =>
        //     {
        //         var taxOrgs = new List<TaxDebtOrg>();
        //         foreach (var taxOrgInfo in debtDto.TaxDebtOrgs)
        //         {
        //         
        //             var taxPayers = new List<TaxDebtPayer>();
        //             foreach (var taxPayerInfo in taxOrgInfo.TaxDebtPayers)
        //             {
        //                 var taxBccs = new List<TaxDebtBcc>();
        //                 foreach (var bccArrearsInfo in taxPayerInfo.TaxDebtBccs)
        //                 {
        //                 
        //                     taxBccs.Add(new TaxDebtBcc
        //                     {
        //                         Bcc = bccArrearsInfo.Bcc,
        //                         Tax = bccArrearsInfo.Tax,
        //                         Total = bccArrearsInfo.Total,
        //                         Poena = bccArrearsInfo.Poena,
        //                         Fine = bccArrearsInfo.Fine,
        //                         CharCode = bccArrearsInfo.CharCode,
        //                         IinBin = bccArrearsInfo.IinBin,
        //                     });
        //                 }
        //                 taxPayers.Add(new TaxDebtPayer
        //                 {
        //                     IinBin = taxPayerInfo.IinBin,
        //                     CharCode = taxPayerInfo.CharCode,
        //                     HeadIinBin = taxPayerInfo.HeadIinBin,
        //                     Total = taxPayerInfo.Total,
        //                     TaxDebtBccs = taxBccs
        //                 });
        //             }
        //             taxOrgs.Add(new TaxDebtOrg
        //             {
        //                 CharCode = taxOrgInfo.CharCode,
        //                 Total = taxOrgInfo.Total,
        //                 TotalTax = taxOrgInfo.TotalTax,
        //                 PensionContribution = taxOrgInfo.PensionContribution,
        //                 SocialContribution = taxOrgInfo.SocialContribution,
        //                 SocialHealthInsurance = taxOrgInfo.SocialHealthInsurance,
        //                 IinBin = taxOrgInfo.IinBin,
        //                 TaxDebtPayers = taxPayers
        //             });
        //         }
        //         var taxDebt = new TaxDebt
        //         {
        //             Total = debtDto.Total,
        //             PensionContribution = debtDto.PensionContribution,
        //             SocialContribution = debtDto.SocialContribution,
        //             SocialHealthInsurance = debtDto.SocialHealthInsurance,
        //             IinBin = debtDto.IinBin,
        //             TaxDebtOrgs = taxOrgs
        //         };
        //         return taxDebt;
        //     });
        // }
    }
}