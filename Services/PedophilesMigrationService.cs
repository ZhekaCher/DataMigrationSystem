
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class PedophilesMigrationService : MigrationService
    {
        private readonly WebPedophilesContext _webPedophilesContext;
        private readonly ParsedPedophilesContext _parsedPedophilesContext;

        public PedophilesMigrationService(int numOfThreats = 1)
        {
            NumOfThreads = numOfThreats;
            _webPedophilesContext = new WebPedophilesContext();
            _parsedPedophilesContext = new ParsedPedophilesContext();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            /*var pedophilesDDto = _parsedPedophilesContext.PedophileDtos
                .Select(x => new Pedophile
                {
                    Id = x.Id,
                    LastName = x.LastName,
                    FirsName = x.FirsName,
                    MiddleName = x.MiddleName,
                    Birthday = x.Birthday,
                    Iin = x.Iin,
                    Court = x.Court,
                    CourtDate =x.CourtDate,
                    CrimeArticle = x.CrimeArticle,
                    Judgement = x.Judgement,
                    JailReleaseDate = x.JailReleaseDate,
                    RelevanceDate = x.RelevanceDate
                });*/
            var pedophilesDDtos = from pedophilesDDto in _parsedPedophilesContext.PedophileDtos
                join indiv in _parsedPedophilesContext.IndividualIins
                    on pedophilesDDto.Iin equals indiv.Code
                    select new Pedophile
                {
                    Id = pedophilesDDto.Id,
                    LastName = pedophilesDDto.LastName,
                    FirsName = pedophilesDDto.FirsName,
                    MiddleName = pedophilesDDto.MiddleName,
                    Birthday = pedophilesDDto.Birthday,
                    Iin = pedophilesDDto.Iin,
                    Court = pedophilesDDto.Court,
                    CourtDate =pedophilesDDto.CourtDate,
                    CrimeArticle = pedophilesDDto.CrimeArticle,
                    Judgement = pedophilesDDto.Judgement,
                    JailReleaseDate = pedophilesDDto.JailReleaseDate,
                    RelevanceDate = pedophilesDDto.RelevanceDate
                };
            foreach (var pedofilDto in pedophilesDDtos)
            {
                await _webPedophilesContext.Pedophiles.Upsert(pedofilDto).On(x => x.Iin).RunAsync();
            }
            var minDate = await _parsedPedophilesContext.PedophileDtos.MinAsync(x => x.RelevanceDate);
            var toDelete = _webPedophilesContext.Pedophiles.Where(x => x.RelevanceDate < minDate);
            _webPedophilesContext.Pedophiles.RemoveRange(toDelete);
            await _webPedophilesContext.SaveChangesAsync();
//            await _parsedPedophilesContext.Database.ExecuteSqlRawAsync("truncate avroradata.pedophiles");
        }
    }
}