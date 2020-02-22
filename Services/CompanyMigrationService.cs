using System;
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
        private readonly WebCompanyContext _webCompanyContext;
        private readonly ParsedCompanyContext _parsedCompanyContext;

        public CompanyMigrationService()
        {
            _webCompanyContext = new WebCompanyContext();
            _parsedCompanyContext = new ParsedCompanyContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        
        public override async Task StartMigratingAsync()
        {
            var companyDtos = from companyDto in _parsedCompanyContext.CompanyDtos
                select DtoToEntity(companyDto);
            foreach (var company in companyDtos)
            {
                await _webCompanyContext.Upsert(company).On(x => x.Bin).RunAsync();
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
                                Type = 2,
                            });
                        }
                    }
                }
                return company;
        }
    }
}