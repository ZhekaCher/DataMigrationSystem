using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class WantedIndividualMigrationService : MigrationService
    {
        private readonly WebWantedIndividualContext _webWantedIndividualContext;
        private readonly ParsedWantedIndividualContext _parsedWantedIndividualContext;

        public WantedIndividualMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            _webWantedIndividualContext = new WebWantedIndividualContext();
            _parsedWantedIndividualContext = new ParsedWantedIndividualContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            var wantedIndividualsDtos = _parsedWantedIndividualContext.WantedIndividualDtos;
            foreach (var wantedIndividualDto in wantedIndividualsDtos)
            {
                var wantedIn = await DtoToEntity(wantedIndividualDto);
                await _webWantedIndividualContext.WantedIndividuals.Upsert(wantedIn).On(x => new {x.Iin,x.ListId}).RunAsync();
            }
            var minDate = await _parsedWantedIndividualContext.WantedIndividualDtos.MinAsync(x => x.RelevanceDate);
            _webWantedIndividualContext.WantedIndividuals.RemoveRange(_webWantedIndividualContext.WantedIndividuals.Where(x=>x.RelevanceDate<minDate));
            await _webWantedIndividualContext.SaveChangesAsync();
            await _parsedWantedIndividualContext.Database.ExecuteSqlRawAsync("truncate avroradata.wanted_individuals restart identity;");

        }

        private async Task MigrateReferences()
        {
            var nationalities = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.Nationality)
                .Distinct();
            foreach (var distinct in nationalities)
            {
                
                await _webWantedIndividualContext.Nationalities.Upsert(new Nationality
                {
                    Name = distinct
                }).On(x=>x.Name).RunAsync();
                
            }
            
            var raceTypes = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.Race).Distinct();
            foreach (var distinct in raceTypes)
            {
                await _webWantedIndividualContext.RaceTypes.Upsert(new RaceType
                {
                    Name = distinct
                }).On(x=>x.Name).RunAsync();
            }

            var issueds = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.IssuedBy).Distinct();
            foreach (var distinct in issueds)
            {
                await _webWantedIndividualContext.Issueds.Upsert(new Issued()
                {
                    Name = distinct
                }).On(x=>x.Name).RunAsync();
            }

            var documents = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.DocumentType).Distinct();
            foreach (var distinct in documents)
            {
                await _webWantedIndividualContext.Documents.Upsert(new Document()
                {
                    Name = distinct
                }).On(x=>x.Name).RunAsync();
            }

            var listTypes = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.List).Distinct();
            foreach (var distinct in listTypes)
            {
                
                await _webWantedIndividualContext.ListTypes.Upsert(new ListType()
                {
                    Name = distinct
                }).On(x=>x.Name).RunAsync();
                
            }
        }

        private async Task<WantedIndividual> DtoToEntity(WantedIndividualDto wantedIndividualDto)
        {
            var wantedIndividual = new WantedIndividual
            {
                Iin = wantedIndividualDto.Iin,
                LastName = wantedIndividualDto.LastName,
                FirstName = wantedIndividualDto.FirstName,
                MiddleName = wantedIndividualDto.MiddleName,
                CodeOfDocument = wantedIndividualDto.CodeOfDocument,
                IssueDate = wantedIndividualDto.IssueDate,
                AuthotityPhone = wantedIndividualDto.AuthotityPhone,
                ReceptionPhone = wantedIndividualDto.ReceptionPhone,
                Birthday = wantedIndividualDto.Birthday,
                SearchingAuthority = wantedIndividualDto.SearchingAuthority,
                SearchingReason = wantedIndividualDto.SearchingAuthority,
                RelevanceDate = wantedIndividualDto.RelevanceDate
            };
            if (wantedIndividualDto.Gender != null)
            {
                if (wantedIndividualDto.Gender=="Мужской")
                {
                    wantedIndividual.Gender = 0;
                }
                else if (wantedIndividualDto.Gender == "Женский")
                {
                    wantedIndividual.Gender = 1;
                }else
                {
                    wantedIndividual.Gender = 2;
                }
            }
            else
            {
                wantedIndividual.Nationality = null;
            }
            if (wantedIndividualDto.Nationality != null)
            {
                wantedIndividual.Nationality =
                    (await _webWantedIndividualContext.Nationalities.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.Nationality))?.Id;
            }
            else
            {
                wantedIndividual.Nationality = null;
            }
            if (wantedIndividualDto.Race != null)
            {
                wantedIndividual.Race =
                    (await _webWantedIndividualContext.RaceTypes.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.Race))?.Id;
            }
            else
            {
                wantedIndividual.Race = null;
            }
            if (wantedIndividualDto.DocumentType != null)
            {
                wantedIndividual.DocumentType =
                    (await _webWantedIndividualContext.Documents.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.DocumentType))?.Id;
            }
            else
            {
                wantedIndividual.DocumentType = null;
            }
            if (wantedIndividualDto.IssuedBy != null)
            {
                wantedIndividual.IssuedBy =
                    (await _webWantedIndividualContext.Issueds.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.IssuedBy))?.Id;
            }
            else
            {
                wantedIndividual.IssuedBy = null;
            }
            if (wantedIndividualDto.List != null)
            {
                wantedIndividual.ListId =
                    (await _webWantedIndividualContext.ListTypes.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.List))?.Id;
            }
            else
            {
                wantedIndividual.ListId = null;
            }
            return wantedIndividual;
        }
    }
}