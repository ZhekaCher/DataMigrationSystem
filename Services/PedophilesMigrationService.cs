using System;
using System.Globalization;
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
            var pedophilesDDto = _parsedPedophilesContext.PedophileDtos
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
                    JailReleaseDate = x.JailReleaseDate
                });
            foreach (var pedofilDto in pedophilesDDto)
            {
                await _webPedophilesContext.Pedophiles.Upsert(pedofilDto).On(x => x.Iin).RunAsync();
            }
        }
    }
}