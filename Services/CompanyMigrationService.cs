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
        public CompanyMigrationService(int numOfThreads = 1)
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
                // await _webCompanyContext.CompaniesOkeds.UpsertRange(company.CompanyOkeds)
                // .On(x => new {x.CompanyId, x.OkedId}).RunAsync();
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

            var katos = parsedCompanyContext.CompanyDtos
                .Select(x => new {x.KatoCode, x.SettlementNameKz, x.SettlementNameRu}).Distinct();
            foreach (var distinct in katos)
            {
                await webCompanyContext.Katos.Upsert(new Kato
                {
                    Id = distinct.KatoCode,
                    NameKz = distinct.SettlementNameKz,
                    NameRu = distinct.SettlementNameRu,
                                    
                }).On(x=>x.Id).RunAsync();
            }
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
                    new CompanyOked {CompanyId = dto.Bin, OkedId = dto.OkedCode, Type = 1}
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