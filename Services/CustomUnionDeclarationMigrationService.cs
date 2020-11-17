using System;
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
    public class CustomUnionDeclarationMigrationService : MigrationService
    {
        private readonly WebCustomUnionDeclarationsContext _webCustomUnionDeclarationsContext;
        private readonly ParsedCustomUnionDeclarations _parsedCustomUnionDeclarations;
        private readonly object _forLock;
        private int _counter;

        public CustomUnionDeclarationMigrationService(int numOfThreads = 5) 
        {
            _webCustomUnionDeclarationsContext = new WebCustomUnionDeclarationsContext();
            _parsedCustomUnionDeclarations = new ParsedCustomUnionDeclarations();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
                if (tasks.Count < NumOfThreads) continue;
                await Task.WhenAny(tasks);
                tasks.RemoveAll(x => x.IsCompleted);
            }
            await Task.WhenAll(tasks);

            await using var parsedCustomUnionDeclarations = new ParsedCustomUnionDeclarations();
            await parsedCustomUnionDeclarations.Database.ExecuteSqlRawAsync("truncate avroradata.custom_union_declarations_ad,avroradata.custom_union_declarations restart identity;");
        }

        private async Task Migrate(int threadNum)
        {
            var parseCud = _parsedCustomUnionDeclarations.CustomUnionDeclarationsDtos;
            foreach (var customUnionDeclarationsDto in parseCud.Where(x=>x.Id % NumOfThreads == threadNum))
            {
                var cUnDic = new CustomUnionDeclarations
                {
                    CertOrgAddress = customUnionDeclarationsDto.CertOrgAddress,
                    CertOrgHeader = customUnionDeclarationsDto.CertOrgHeader,
                    CertOrgName = customUnionDeclarationsDto.CertOrgName,
                    DeclarantsAddress = customUnionDeclarationsDto.DeclarantsAddress,
                    DeclarantsIinBiin = customUnionDeclarationsDto.DeclarantsIinBiin,
                    DeclarantsName = customUnionDeclarationsDto.DeclarantsName,
                    Details = customUnionDeclarationsDto.Details,
                    EvidanceDoc = customUnionDeclarationsDto.EvidanceDoc,
                    ExtensionInform = customUnionDeclarationsDto.ExtensionInform,
                    RegNum = customUnionDeclarationsDto.RegNum,
                    ValFrom = customUnionDeclarationsDto.ValFrom,
                    ValUntil = customUnionDeclarationsDto.ValUntil ,
                    ManufacturersName = customUnionDeclarationsDto.ManufacturersName,
                    ManufacturersAdress = customUnionDeclarationsDto.ManufacturersAdress,
                    ProductName = customUnionDeclarationsDto.ProductName,
                    IdentificateInform = customUnionDeclarationsDto.IdentificateInform,
                    TnCode = customUnionDeclarationsDto.TnCode,
                    Npa = customUnionDeclarationsDto.Npa,
                    RelevanceDate = customUnionDeclarationsDto.RelevanceDate
                };
                await _webCustomUnionDeclarationsContext.CustomUnionDeclarationsAds.AddRangeAsync(
                    _webCustomUnionDeclarationsContext.CustomUnionDeclarationsAds.Select(x =>
                        new CustomUnionDeclarationsAd
                        {
                            Name = x.Name,
                            CtRk = x.CtRk,
                            TnVad = x.TnVad,
                            Declarations = x.Declarations,
                            RelevanceDate = x.RelevanceDate
                        }));
                await _webCustomUnionDeclarationsContext.CustomUnionDeclarationses.Upsert(cUnDic).On(x => x.RegNum)
                   .RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
                
            }
        }
    }
}