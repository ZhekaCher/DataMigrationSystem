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
    public class CompanyMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _total;
        public CompanyMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
            using var parsedCompanyContext = new ParsedCompanyContext(); 
            _total = parsedCompanyContext.CompanyDtos.Count();
            _forLock = new object();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        
        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            Logger.Warn(NumOfThreads);
            Logger.Info("Start");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(MigrateAsync(i));

            await Task.WhenAll(tasks);
            await using var parsedCompanyContext = new ParsedCompanyContext();
            await parsedCompanyContext.Database.ExecuteSqlRawAsync(
                "insert into avroradata.company_bin (code) select bin from avroradata.company a where not exists(select from avroradata.company_bin b where a.bin = b.code);");
            await parsedCompanyContext.Database.ExecuteSqlRawAsync("truncate avroradata.company restart identity;");
            Logger.Info("Ended");
        }

        private async Task MigrateAsync(int threadNum)
        {
            await using var webCompanyContext = new WebCompanyContext();
            await using var parsedCompanyContext = new ParsedCompanyContext();
            var companyDtos = from companyDto in parsedCompanyContext.CompanyDtos where companyDto.Id % NumOfThreads == threadNum
                select companyDto;
            foreach (var companyDto in companyDtos)
            {
                var company = DtoToEntity(companyDto);
                await webCompanyContext.Companies.Upsert(company).On(x => x.Bin).RunAsync();
                foreach (var oked in company.CompanyOkeds.Where(x=>x.OkedId!="0"))
                {
                    await webCompanyContext.CompaniesOkeds.Upsert(oked).On(x=>new {x.CompanyId, x.OkedId}).RunAsync();
                }
                lock (_forLock)
                {
                    Logger.Trace(_total--);
                }
            }
        }

        private async Task MigrateReferences()
        {
            await using var webCompanyContext = new WebCompanyContext();
            await using var parsedCompanyContext = new ParsedCompanyContext();
            var krps = parsedCompanyContext.CompanyDtos
                .Select(x => new {x.KrpCode, x.KrpNameKz, x.KrpNameRu}).Distinct();
            foreach (var distinct in krps)
            {
                await webCompanyContext.Krps.Upsert(new Krp
                {
                    Id = distinct.KrpCode,
                    NameKz = distinct.KrpNameKz,
                    NameRu = distinct.KrpNameRu,
                                    
                }).On(x=>x.Id).RunAsync();
            }
            var secondOkeds = parsedCompanyContext.CompanyDtos.Select(x => new {x.SecondOkedCode}).Distinct();
            foreach (var secondOked in secondOkeds)
            {
                if (secondOked.SecondOkedCode != null)
                {
                    var secOkeds = secondOked.SecondOkedCode.Split(',');
                    foreach (var oked in secOkeds)
                    {
                        await webCompanyContext.Okeds.Upsert(
                            new Oked
                            {
                                Id = oked,
                                NameKz = "",
                                NameRu = "",
                            }).On(x => x.Id).RunAsync();
                    }
                }
            }
            var okeds = parsedCompanyContext.CompanyDtos
                .Select(x => new {x.OkedCode, x.ActivityNameKz, x.ActivityNameRu}).Distinct();
            foreach (var distinct in okeds)
            {
                await webCompanyContext.Okeds.Upsert(
                    new Oked
                    {
                        Id = distinct.OkedCode,
                        NameKz = distinct.ActivityNameKz,
                        NameRu = distinct.ActivityNameRu,
                    }).On(x=>x.Id).RunAsync();
            }
        }
        private Company DtoToEntity(CompanyDto dto)
        {
            var company = new Company
            {
                Bin = dto.Bin,
                IdRegion = dto.Region,
                NameRu = dto.NameRu,
                NameKz = dto.NameKz,
                DateRegistration = dto.RegistrationDate,
                IdKrp = dto.KrpCode,
                IdKato = dto.KatoCode,
                LegalAddress = dto.LegalAddress,
                FullnameDirector = dto.NameHead,
                RelevanceDate = dto.RelevanceDate
            };
            if (dto.OkedCode != null)
            {
                company.CompanyOkeds = new List<CompanyOked>
                {
                    new CompanyOked
                    {
                        CompanyId = dto.Bin, OkedId = dto.OkedCode, Type = 1
                    }
                };
                if (dto.SecondOkedCode != null)
                {
                    var okeds = dto.SecondOkedCode.Split(',');
                    foreach (var oked in okeds)
                    {
                        company.CompanyOkeds.Add(new CompanyOked
                        {
                            CompanyId = dto.Bin,
                            OkedId = oked,
                            Type = 2
                        });
                    }
                }
            }
            return company;
        }
    }
}