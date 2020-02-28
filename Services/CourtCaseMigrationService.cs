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
    public class CourtCaseMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter;
        public CourtCaseMigrationService(int numOfThreads = 30)
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
            var tasks = new List<Task>();

            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
        }

        private async Task Migrate(int numThread)
        {
            await using var parsedCourtCaseContext = new ParsedCourtCaseContext();
            await using var webCourtCaseContext = new WebCourtCaseContext();
            var courtCaseDtos = parsedCourtCaseContext.CourtCaseDtos.Where(x=>x.Id%NumOfThreads == numThread);
            foreach (var courtCaseDto in courtCaseDtos)
            {
                var courtCase = await DtoToEntity(courtCaseDto, webCourtCaseContext);
                await webCourtCaseContext.CourtCases.Upsert(courtCase).On(x => x.Number).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
            
            var companyDtos = from courtCaseEntityDto in parsedCourtCaseContext.CourtCaseEntityDtos 
                where courtCaseEntityDto.Id%NumOfThreads == numThread
                select courtCaseEntityDto;
            foreach (var companyDto in companyDtos)
            {
                await webCourtCaseContext.CompanyCourtCaseEntities.Upsert(new CompanyCourtCaseEntity
                {
                    IinBin = companyDto.IinBin,
                    Number = companyDto.Number
                }).On(x => new{x.Number, x.IinBin}).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
        private async Task MigrateReferences()
        {
            await using var parsedCourtCaseContext = new ParsedCourtCaseContext();
            await using var webCourtCaseContext = new WebCourtCaseContext();
            var courts = parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Court).Distinct();
            foreach (var distinct in courts)
            {
                await webCourtCaseContext.Courts.Upsert(new Court
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
            
            var caseTypes = parsedCourtCaseContext.CourtCaseDtos.Select(x => x.CaseType).Distinct();
            foreach (var distinct in caseTypes)
            {
                await webCourtCaseContext.CaseTypes.Upsert(new CourtCaseType
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }

            var documentTypes = parsedCourtCaseContext.CourtCaseDtos.Select(x => x.DocumentType).Distinct();
            foreach (var distinct in documentTypes)
            {
                await webCourtCaseContext.DocumentTypes.Upsert(new CourtCaseDocumentType
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }

            await webCourtCaseContext.SaveChangesAsync();
            var categories = parsedCourtCaseContext.CourtCaseDtos.Select(x => x.Category).Distinct();
            foreach (var distinct in categories)
            {
                await webCourtCaseContext.CourtCaseCategories.Upsert(new CourtCaseCategory
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
        }

        private async Task<CourtCase> DtoToEntity(CourtCaseDto caseDto, WebCourtCaseContext webCourtCaseContext)
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
                    (await webCourtCaseContext.Courts.FirstOrDefaultAsync(x => x.Name == caseDto.Court))
                    ?.Id;
            }
            else
            {
                courtCase.CourtId = null;
            }

            if (caseDto.CaseType != null)
            {
                courtCase.CaseTypeId =
                    (await webCourtCaseContext.CaseTypes.FirstOrDefaultAsync(x => x.Name == caseDto.CaseType))
                    ?.Id;
            }
            else
            {
                courtCase.CaseTypeId = null;
            }


            if (caseDto.DocumentType != null)
            {
                courtCase.DocumentTypeId =
                    (await webCourtCaseContext.DocumentTypes.FirstOrDefaultAsync(x => x.Name == caseDto.DocumentType))
                    ?.Id;
            }
            else
            {
                courtCase.DocumentTypeId = null;
            }

            if (caseDto.Category != null)
            {
                courtCase.CategoryId =
                    (await webCourtCaseContext.CourtCaseCategories.FirstOrDefaultAsync(x => x.Name == caseDto.Category)
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