namespace DataMigrationSystem.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DataMigrationSystem.Models.Parsed.Avroradata.CompaniesFl;
    using DataMigrationSystem.Context.Parsed.Avroradata;
    using DataMigrationSystem.Context.Web.Avroradata;
    using DataMigrationSystem.Models.Web.Avroradata;
    using Microsoft.EntityFrameworkCore;

    public class CompaniesFlMigrationService : MigrationService
    {
        public CompaniesFlMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
            using var parsedCompanyFlContext = new ParsedCompaniesFlContext();
            Total = parsedCompanyFlContext.CompaniesFlDtos.Select(o => o.Bin).Count();
        }

        private static int Total { get; set; }

        public override async Task StartMigratingAsync()
        {
            Logger.Warn(NumOfThreads);
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            
            await MigrateReferences();
            
            var tasks = new List<Task>();
            await using var parsedCompanyFlContext = new ParsedCompaniesFlContext();
            var companiesFlDtos = 
                from companiesFlDto in parsedCompanyFlContext.CompaniesFlDtos select companiesFlDto;

            foreach (var companiesFlDto in companiesFlDtos)
            {
                tasks.Add(MigrateAsync(companiesFlDto));
                if (tasks.Count == NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                    tasks.RemoveAll(x => x.IsFaulted);
                }
            }
            await Task.WhenAll(tasks);
            
            await using var webContext = new WebCompanyContext();
            await parsedCompanyFlContext.Database.ExecuteSqlRawAsync(
                "insert into avroradata.company_bin (code) select bin from avroradata.company_from_fl a where not exists(select from avroradata.company_bin b where a.bin = b.code);");
            await webContext.Database.ExecuteSqlRawAsync("DELETE FROM avroradata.companies_oked T1 USING avroradata.companies_oked T2 WHERE T1.ctid < T2.ctid AND T1.c_id = T2.c_id AND T1.type=1 and T2.type=1;");
            await webContext.Database.ExecuteSqlRawAsync("delete from avroradata.oked where i='0';");
            await parsedCompanyFlContext.Database.ExecuteSqlRawAsync("truncate avroradata.company_from_fl restart identity;");
            Logger.Info("End of migration");
        }
        
        private async Task MigrateAsync(CompaniesFlDto companiesFlDto)
        {
            await using var webCompanyContext = new WebCompanyContext();
            
            var webCompany = DtoToEntity(companiesFlDto);
            await webCompanyContext.Companies.Upsert(webCompany).On(x => x.Bin).RunAsync();
            foreach (var oked in webCompany.CompanyOkeds.Where(x => x.OkedId != "0"))
            {
                await webCompanyContext.CompaniesOkeds.Upsert(oked).On(x=>new {x.CompanyId, x.OkedId}).RunAsync();
            }

            Logger.Trace(Total--);
        }

        private async Task MigrateReferences()
        {
            await using var webCompanyContext = new WebCompanyContext();
            await using var parsedCompaniesFlContext = new ParsedCompaniesFlContext();
            var krps = parsedCompaniesFlContext.CompaniesFlDtos
                .Select(x => new {x.KrpCode, x.KrpNameRu}).Distinct();
            foreach (var distinct in krps)
            {
                await webCompanyContext.Krps.Upsert(new Krp
                {
                    Id = distinct.KrpCode,
                    NameRu = distinct.KrpNameRu,
                    NameKz = null,
                }).On(x=>x.Id).RunAsync();
            }
            var secondOkeds = parsedCompaniesFlContext.CompaniesFlDtos.Select(x => new {x.SecondOkedCode}).Distinct();
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
            
            var okeds = parsedCompaniesFlContext.CompaniesFlDtos
                .Select(x => new {x.OkedCode, x.ActivityNameRu}).Distinct();
            foreach (var distinct in okeds)
            {
                await webCompanyContext.Okeds.Upsert(
                    new Oked
                    {
                        Id = distinct.OkedCode,
                        NameRu = distinct.ActivityNameRu,
                        NameKz = null,
                    }).On(x=>x.Id).RunAsync();
            }
            
            var katos = parsedCompaniesFlContext.CompaniesFlDtos
                .Select(x => new {x.KatoCode}).Distinct();
            foreach (var distinct in katos)
            {
                await webCompanyContext.Katos.Upsert(
                    new Kato
                    {
                        Id = distinct.KatoCode,
                        NameRu = null,
                        NameKz = null,
                    }).On(x=>x.Id).NoUpdate().RunAsync();
            }
        }
        private Company DtoToEntity(CompaniesFlDto dto)
        {
            var company = new Company
            {
                Bin = dto.Bin,
                IdRegion = dto.Region,
                NameRu = dto.NameRu,
                DateRegistration = dto.RegistrationDate,
                IdKrp = dto.KrpCode,
                IdKato = dto.KatoCode,
                LegalAddress = dto.KatoAddress,
                FullnameDirector = dto.NameHead,
                RelevanceDate = dto.RelevanceDate
            };

            if (dto.OkedCode != null)
            {
                company.CompanyOkeds = new List<CompanyOked>
                {
                    new CompanyOked
                    {
                        CompanyId = dto.Bin,
                        OkedId = dto.OkedCode, 
                        Type = dto.Ip ? 2 : 1,
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
                            Type = dto.Ip ? 2 : 1,
                        });
                    }
                }
            }
            return company;
        }
    }
}