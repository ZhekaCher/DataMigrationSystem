using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class CourtCaseMigrationService : MigrationService
    {
        private readonly WebCourtCaseContext _webCourtCaseContext;
        private readonly ParsedCourtCaseContext _parsedCourtCaseContext;

        public CourtCaseMigrationService(WebCourtCaseContext webCourtCaseContext, ParsedCourtCaseContext parsedCourtCaseContext)
        {
            _webCourtCaseContext = webCourtCaseContext;
            _parsedCourtCaseContext = parsedCourtCaseContext;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();

        }

        public override async  Task StartMigratingAsync()
        {
            
            await MigrateReferences();
            
            var courtCaseDtos = _parsedCourtCaseContext.CourtCaseDtos;
            foreach (var courtCaseDto in courtCaseDtos)
            {
                var courtCase = await DtoToEntity(courtCaseDto);
                var found = await _webCourtCaseContext.CourtCases.FirstOrDefaultAsync(x => x.Number == courtCase.Number);
                if (found == null)
                {
                    await _webCourtCaseContext.AddAsync(courtCase);
                }
                else
                {
                    _webCourtCaseContext.Entry(found).CurrentValues.SetValues(courtCase);
                }
                await _webCourtCaseContext.SaveChangesAsync();
            }
            
            var companyDtos = from courtCaseEntityDto in _parsedCourtCaseContext.CourtCaseEntityDtos
                join companies in _parsedCourtCaseContext.ParsedCompanies
                    on courtCaseEntityDto.IinBin equals companies.Bin
                orderby courtCaseEntityDto.IinBin
                select courtCaseEntityDto;
            foreach (var companyDto in companyDtos)
            {
                var found = await _webCourtCaseContext.CompanyCourtCaseEntities.FirstOrDefaultAsync(x =>
                    x.IinBin == companyDto.IinBin && x.Number == companyDto.Number);
                if (found == null)
                {
                    await _webCourtCaseContext.CompanyCourtCaseEntities.AddAsync(new CompanyCourtCaseEntity
                    {
                        IinBin = companyDto.IinBin,
                        Number = companyDto.Number
                    });
                }
                await _webCourtCaseContext.SaveChangesAsync();
            }
            
            // var individualDtos = from courtCaseEntityDto in _parsedCourtCaseContext.CourtCaseEntityDtos
            //     join individual in _parsedCourtCaseContext.ParsedIndividuals
            //         on courtCaseEntityDto.IinBin equals individual.Iin
            //     orderby courtCaseEntityDto.IinBin
            //     select courtCaseEntityDto;
            // foreach (var individualDto in individualDtos)
            // {
            //     var found = await _webCourtCaseContext.IndividualCourtCaseEntities.FirstOrDefaultAsync(x =>
            //         x.IinBin == individualDto.IinBin && x.Number == individualDto.Number);
            //     if (found == null)
            //     {
            //         await _webCourtCaseContext.IndividualCourtCaseEntities.AddAsync(new IndividualCourtCaseEntity
            //         {
            //             IinBin = individualDto.IinBin,
            //             Number = individualDto.Number
            //         });
            //     }
            //     else
            //     {
            //         found.RelevanceDate = individualDto.RelevanceDate;
            //     }
            //     await _webCourtCaseContext.SaveChangesAsync();
            // }
        }

        private async Task MigrateReferences()
        {
            var courts = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Court).Distinct();
            foreach (var distinct in courts)
            {
                var found = await _webCourtCaseContext.Courts.FirstOrDefaultAsync(x=>x.Name == distinct);
                if (found == null)
                {
                    await _webCourtCaseContext.Courts.AddAsync(new Court
                    {
                        Name = distinct
                    });
                }
            }

            await _webCourtCaseContext.SaveChangesAsync();
            var caseTypes = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.CaseType).Distinct();
            foreach (var distinct in caseTypes)
            {
                var found = await _webCourtCaseContext.CaseTypes.FirstOrDefaultAsync(x=>x.Name == distinct);
                if (found == null)
                {
                    await _webCourtCaseContext.CaseTypes.AddAsync(new CourtCaseType()
                    {
                        Name = distinct
                    });
                }
            }
            await _webCourtCaseContext.SaveChangesAsync();
            var documentTypes = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.DocumentType).Distinct();
            foreach (var distinct in documentTypes)
            {
                var found = await _webCourtCaseContext.DocumentTypes.FirstOrDefaultAsync(x=>x.Name == distinct);
                if (found == null)
                {
                    await _webCourtCaseContext.DocumentTypes.AddAsync(new CourtCaseDocumentType
                    {
                        Name = distinct
                    });
                }
            }
            await _webCourtCaseContext.SaveChangesAsync();
            var categories = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Category).Distinct();
            foreach (var distinct in categories)
            {
                var found = await _webCourtCaseContext.CourtCaseCategories.FirstOrDefaultAsync(x=>x.Name == distinct);
                if (found == null)
                {
                    await _webCourtCaseContext.CourtCaseCategories.AddAsync(new CourtCaseCategory
                    {
                        Name = distinct
                    });
                }
            }
            await _webCourtCaseContext.SaveChangesAsync();
        }
        private async Task<CourtCase> DtoToEntity(CourtCaseDto caseDto)
        {
            var courtCase = new CourtCase
            {
                Number = caseDto.Number,
                Date = caseDto.Date,
                Result = caseDto.Result,
                Sides = caseDto.Sides,
                FileName = caseDto.FileName
            };
            if (caseDto.Court != null)
            {
                courtCase.CourtId = (await _webCourtCaseContext.Courts.FirstOrDefaultAsync(x => x.Name == caseDto.Court))?.Id;
            }
            else
            {
                courtCase.CourtId = null;
            }
            
            if (caseDto.CaseType != null)
            {
                courtCase.CaseTypeId = (await _webCourtCaseContext.CaseTypes.FirstOrDefaultAsync(x => x.Name == caseDto.CaseType))?.Id;
            }
            else
            {
                courtCase.CaseTypeId = null;
            }
            
            
            if (caseDto.DocumentType != null)
            {
                courtCase.DocumentTypeId = (await _webCourtCaseContext.DocumentTypes.FirstOrDefaultAsync(x => x.Name == caseDto.DocumentType))?.Id;
            }
            else
            {
                courtCase.DocumentTypeId = null;
            }
            
            if (caseDto.Category != null)
            {
                courtCase.CategoryId = (await _webCourtCaseContext.CourtCaseCategories.FirstOrDefaultAsync(x => x.Name == caseDto.Category))?.Id;
            }
            else
            {
                courtCase.CategoryId = null;
            }
            
            return courtCase;
        }
    }
}