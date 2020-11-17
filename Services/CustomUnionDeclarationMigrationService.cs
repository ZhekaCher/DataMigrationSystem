using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NLog;

namespace DataMigrationSystem.Services
{
    public class CustomUnionDeclarationMigrationService : MigrationService
    {
        private readonly WebCustomUnionDeclarationsContext _webCustomUnionDeclarationsContext;
        private readonly ParsedCustomUnionDeclarations _parsedCustomUnionDeclarations;
        private readonly object _forLock;
        private int _counter;

        public CustomUnionDeclarationMigrationService(int numOfThreads = 10) 
        {
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        public override async Task StartMigratingAsync()
        {

            await Migrate();
            await using var parsedCustomUnionDeclarations = new ParsedCustomUnionDeclarations();
            await parsedCustomUnionDeclarations.Database.ExecuteSqlRawAsync("truncate avroradata.custom_union_declarations_ad,avroradata.custom_union_declarations restart identity;");
        }
        
        
        private async Task Migrate()
        {
            var tasks = new List<Task>();
            await using var parsed = new ParsedCustomUnionDeclarations();
            var ads = parsed.CustomUnionDeclarationsDtos
                .AsNoTracking()
                .Include(x => x.CustomUnionDeclarationsAdDto);
            
            foreach (var ad in ads)
            {
                tasks.Add(Insert(ad));
                if (tasks.Count < NumOfThreads) continue;
                await Task.WhenAny(tasks);
                tasks.RemoveAll(x => x.IsCompleted);
            }
            await Task.WhenAll(tasks);
        }

        private async Task Insert(CustomUnionDeclarationsDto declarationsDto)
        {
            await using var web = new WebCustomUnionDeclarationsContext();
            web.ChangeTracker.AutoDetectChangesEnabled = false;
            await web.CustomUnionDeclarationses.Upsert(new CustomUnionDeclarations
            {
                CertOrgAddress = declarationsDto.CertOrgAddress,
                CertOrgHeader = declarationsDto.CertOrgHeader,
                CertOrgName = declarationsDto.CertOrgName,
                DeclarantsAddress = declarationsDto.DeclarantsAddress,
                DeclarantsIinBiin = declarationsDto.DeclarantsIinBiin,
                DeclarantsName = declarationsDto.DeclarantsName,
                Details = declarationsDto.Details,
                EvidanceDoc = declarationsDto.EvidanceDoc,
                ExtensionInform = declarationsDto.ExtensionInform,
                RegNum = declarationsDto.RegNum,
                ValFrom = declarationsDto.ValFrom,
                ValUntil = declarationsDto.ValUntil,
                ManufacturersName = declarationsDto.ManufacturersName,
                ManufacturersAdress = declarationsDto.ManufacturersAdress,
                ProductName = declarationsDto.ProductName,
                IdentificateInform = declarationsDto.IdentificateInform,
                TnCode = declarationsDto.TnCode,
                Npa = declarationsDto.Npa,
                RelevanceDate = declarationsDto.RelevanceDate
            }).On(x => x.RegNum).RunAsync();
            web.CustomUnionDeclarationsAds.RemoveRange(web.CustomUnionDeclarationsAds.Where(x=>x.Declarations == declarationsDto.RegNum));
            await web.SaveChangesAsync();
            if (declarationsDto.CustomUnionDeclarationsAdDto != null)
            {
                await web.CustomUnionDeclarationsAds.AddRangeAsync(declarationsDto.CustomUnionDeclarationsAdDto.Select(
                    x => new CustomUnionDeclarationsAd
                    {
                        Name = x.Name,
                        CtRk = x.CtRk,
                        TnVad = x.TnVad,
                        Declarations = x.Declarations,
                        RelevanceDate = x.RelevanceDate
                    }));
                await web.SaveChangesAsync();
            }
            lock (_forLock)
            {
                Logger.Trace(_counter++);
            }
        }
    }
}