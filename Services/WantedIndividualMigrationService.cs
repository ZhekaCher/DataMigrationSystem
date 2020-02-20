using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Models;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class WantedIndividualMigrationService : MigrationService
    {
        private readonly WebWantedIndividualContext _webWantedIndividualContext;
        private readonly ParsedWantedIndividualContext _parsedWantedIndividualContext;

        public WantedIndividualMigrationService(WebWantedIndividualContext webWantedIndividualContext,
            ParsedWantedIndividualContext parsedWantedIndividualContext)
        {
            _webWantedIndividualContext = webWantedIndividualContext;
            _parsedWantedIndividualContext = parsedWantedIndividualContext;
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
                var found = await _webWantedIndividualContext.WantedIndividuals.FirstOrDefaultAsync(x =>
                    x.Iin == wantedIn.Iin);
                if (found==null)
                {
                    await _webWantedIndividualContext.AddAsync(wantedIn);
                }
                else
                {
                    _webWantedIndividualContext.Entry(found).CurrentValues.SetValues(wantedIn);
                }

                await _webWantedIndividualContext.SaveChangesAsync();
            }
        }

        private async Task MigrateReferences()
        {
            var nationalities = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.Nationality)
                .Distinct();
            foreach (var distinct in nationalities)
            {
                var found = await _webWantedIndividualContext.Nationalities.FirstOrDefaultAsync(x =>
                    x.Name == distinct);
                if (found == null)
                {
                    await _webWantedIndividualContext.AddAsync(new Nationality()
                    {
                        Name = distinct
                    });
                }
            }
            await _webWantedIndividualContext.SaveChangesAsync();
            
            var raceTypes = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.Race).Distinct();
            foreach (var distinct in raceTypes)
            {
                var found = await _webWantedIndividualContext.RaceTypes.FirstOrDefaultAsync(x => x.Name == distinct);
                if (found == null)
                {
                    await _webWantedIndividualContext.RaceTypes.AddAsync(new RaceType()
                    {
                        Name = distinct
                    });
                }
            }
            await _webWantedIndividualContext.SaveChangesAsync();

            var issueds = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.IssuedBy).Distinct();
            foreach (var distinct in issueds)
            {
                var found = await _webWantedIndividualContext.Issueds.FirstOrDefaultAsync(x => x.Name == distinct);
                if (found == null)
                {
                    await _webWantedIndividualContext.Issueds.AddAsync(new Issued()
                    {
                        Name = distinct
                    });
                }
            }
            await _webWantedIndividualContext.SaveChangesAsync();

            var documents = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.DocumentType).Distinct();
            foreach (var distinct in documents)
            {
                var found = _webWantedIndividualContext.Documents.FirstOrDefaultAsync(x => x.Name == distinct);
                if (found == null)
                {
                    await _webWantedIndividualContext.Documents.AddAsync(new Document()
                    {
                        Name = distinct
                    });
                }
            }
            await _webWantedIndividualContext.SaveChangesAsync();

            var listTypes = _parsedWantedIndividualContext.WantedIndividualDtos.Select(x => x.List).Distinct();
            foreach (var distinct in listTypes)
            {
                var found = _webWantedIndividualContext.ListTypes.FirstOrDefaultAsync(x => x.Name == distinct);
                if (found == null)
                {
                    await _webWantedIndividualContext.ListTypes.AddAsync(new ListType()
                    {
                        Name = distinct
                    });
                }
            }
            await _webWantedIndividualContext.SaveChangesAsync();
        }

        private async Task<WantedIndividual> DtoToEntity(WantedIndividualDto wantedIndividualDto)
        {
            var wantedIndividual = new WantedIndividual
            {
                Iin = wantedIndividualDto.Iin,
                LastName = wantedIndividualDto.LastName,
                FirstName = wantedIndividualDto.FirstName,
                MiddleName = wantedIndividualDto.MiddleName,
                Gender = wantedIndividualDto.Gender,
                CodeOfDocument = wantedIndividualDto.CodeOfDocument,
                IssueDate = wantedIndividualDto.IssueDate,
                AuthotityPhone = wantedIndividualDto.AuthotityPhone,
                ReceptionPhone = wantedIndividualDto.ReceptionPhone,
                Birthday = wantedIndividualDto.Birthday,
                SearchingAuthority = wantedIndividualDto.SearchingAuthority,
                SearchingReason = wantedIndividualDto.SearchingAuthority
            };

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
                wantedIndividual.List =
                    (await _webWantedIndividualContext.ListTypes.FirstOrDefaultAsync(x =>
                        x.Name == wantedIndividualDto.List))?.Id;
            }
            else
            {
                wantedIndividual.List = null;
            }
            return wantedIndividual;
        }
    }
}