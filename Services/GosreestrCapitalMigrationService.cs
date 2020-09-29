using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GosreestrCapitalMigrationService : MigrationService
    {
        private int _total;

        private readonly object _lock = new object();

        public GosreestrCapitalMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webGosreestrCapitalContext = new WebGosreestrCapitalContext();
            await using var parsedGosreestrCapitalContext = new ParsedGosreestrCapitalContext();
            _total = parsedGosreestrCapitalContext.GosreestrBinDtos.Count();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            await Migrate();

            await parsedGosreestrCapitalContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.gosreestr_authorized_capitals restart identity cascade;");
            await parsedGosreestrCapitalContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.gosreestr_bins restart identity cascade;");
        }

        private async Task Migrate()
        {
            Logger.Info("started thread");
            await using var parsedGosreestrCapitalContext = new ParsedGosreestrCapitalContext();
            var gosreestrCapitalDtos = parsedGosreestrCapitalContext.GosreestrBinDtos.AsNoTracking()
                .Include(x => x.GosreestrCapitalDto);
            var tasks = new List<Task>();
            foreach (var dto in gosreestrCapitalDtos)
            {
                tasks.Add(Insert(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task Insert(GosreestrBinDto dto)
        {
            await using var webGosreestrCapitalContext = new WebGosreestrCapitalContext();
            webGosreestrCapitalContext.ChangeTracker.AutoDetectChangesEnabled = false;
            try
            {
                var gosreestrBin = new GosreestrBin
                {
                    Bin = dto.Bin,
                    GovParticipation = dto.GovParticipation
                };
                await webGosreestrCapitalContext.GosreestrBins.Upsert(gosreestrBin).On(x => x.Bin).RunAsync();
                if (dto.GosreestrCapitalDto != null)
                {
                    var gosreestrCapital = new GosreestrCapital
                    {
                        Bin = dto.GosreestrCapitalDto.Bin,
                        Capital = dto.GosreestrCapitalDto.Capital,
                        GovContribution = dto.GosreestrCapitalDto.GovContribution,
                        GovParticipation = dto.GosreestrCapitalDto.GovParticipation,
                        GovPackage = dto.GosreestrCapitalDto.GovPackage,
                        Registrar = dto.GosreestrCapitalDto.Registrar,
                        FreeContribution = dto.GosreestrCapitalDto.FreeContribution,
                        FreeShares = dto.GosreestrCapitalDto.FreeShares,
                        SharesEncumbered = dto.GosreestrCapitalDto.SharesEncumbered,
                    };

                    await webGosreestrCapitalContext.GosreestrCapitals.Upsert(gosreestrCapital).On(x => x.Bin)
                        .RunAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }
    }
}