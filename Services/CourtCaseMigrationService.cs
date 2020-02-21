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
    public class CourtCaseMigrationService : MigrationService
    {
        private readonly WebCourtCaseContext _webCourtCaseContext;
        private readonly ParsedCourtCaseContext _parsedCourtCaseContext;

        public CourtCaseMigrationService()
        {
            _webCourtCaseContext = new WebCourtCaseContext();
            _parsedCourtCaseContext = new ParsedCourtCaseContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();

            long i = 0;
            var courtCaseDtos = _parsedCourtCaseContext.CourtCaseDtos;
            foreach (var courtCaseDto in courtCaseDtos)
            {
                var courtCase = await DtoToEntity(courtCaseDto);
                await _webCourtCaseContext.CourtCases.Upsert(courtCase).On(x => x.Number).RunAsync();
                Logger.Trace(i++);
            }

            var companyDtos = from courtCaseEntityDto in _parsedCourtCaseContext.CourtCaseEntityDtos
                join companies in _parsedCourtCaseContext.ParsedCompanies
                    on courtCaseEntityDto.IinBin equals companies.Bin
                orderby courtCaseEntityDto.IinBin
                select courtCaseEntityDto;
            foreach (var companyDto in companyDtos)
            {
                await _webCourtCaseContext.CompanyCourtCaseEntities.Upsert(new CompanyCourtCaseEntity
                {
                    IinBin = companyDto.IinBin,
                    Number = companyDto.Number
                }).On(x => new{x.Number, x.IinBin}).RunAsync();
            }
        }

        private async Task MigrateReferences()
        {
            var courts = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Court).Distinct();
            foreach (var distinct in courts)
            {
                await _webCourtCaseContext.Courts.Upsert(new Court
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
            
            var caseTypes = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.CaseType).Distinct();
            foreach (var distinct in caseTypes)
            {
                await _webCourtCaseContext.CaseTypes.Upsert(new CourtCaseType
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }

            var documentTypes = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.DocumentType).Distinct();
            foreach (var distinct in documentTypes)
            {
                await _webCourtCaseContext.DocumentTypes.Upsert(new CourtCaseDocumentType
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }

            await _webCourtCaseContext.SaveChangesAsync();
            var categories = _parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Category).Distinct();
            foreach (var distinct in categories)
            {
                await _webCourtCaseContext.CourtCaseCategories.Upsert(new CourtCaseCategory
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
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
                courtCase.CourtId =
                    (await _webCourtCaseContext.Courts.FirstOrDefaultAsync(x => x.Name == caseDto.Court))
                    ?.Id;
            }
            else
            {
                courtCase.CourtId = null;
            }

            if (caseDto.CaseType != null)
            {
                courtCase.CaseTypeId =
                    (await _webCourtCaseContext.CaseTypes.FirstOrDefaultAsync(x => x.Name == caseDto.CaseType))
                    ?.Id;
            }
            else
            {
                courtCase.CaseTypeId = null;
            }


            if (caseDto.DocumentType != null)
            {
                courtCase.DocumentTypeId =
                    (await _webCourtCaseContext.DocumentTypes.FirstOrDefaultAsync(x => x.Name == caseDto.DocumentType))
                    ?.Id;
            }
            else
            {
                courtCase.DocumentTypeId = null;
            }

            if (caseDto.Category != null)
            {
                courtCase.CategoryId =
                    (await _webCourtCaseContext.CourtCaseCategories.FirstOrDefaultAsync(x => x.Name == caseDto.Category)
                    )?.Id;
            }
            else
            {
                courtCase.CategoryId = null;
            }

            return courtCase;
        }
    }
}