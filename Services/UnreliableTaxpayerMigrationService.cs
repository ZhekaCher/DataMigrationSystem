using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class UnreliableTaxpayerMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _count;

        public UnreliableTaxpayerMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }

        public override async Task StartMigratingAsync()
        {
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            _count = await parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.CountAsync();
            await MigrateAsync();

            await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
            var minDate = await parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos.MinAsync(x => x.RelevanceDate);
            await webUnreliableTaxpayerContext.Database.ExecuteSqlInterpolatedAsync(
                $"delete from avroradata.unreliable_taxpayers where relevance_date<{minDate}");
            await parsedUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.unreliable_taxpayers;");
            await webUnreliableTaxpayerContext.Database.ExecuteSqlRawAsync(
                $"call avroradata.unreliable_companies_updater();");
        }

        private async Task Insert(UnreliableTaxpayerDto unreliableTaxpayer)
        {
            //Inserting unreliable taxpayer
            try
            {
                await using var webUnreliableTaxpayerContext = new WebUnreliableTaxpayerContext();
                webUnreliableTaxpayerContext.ChangeTracker.AutoDetectChangesEnabled = false;
                await webUnreliableTaxpayerContext.UnreliableTaxpayers.Upsert(
                        new UnreliableTaxpayer
                        {
                            RelevanceDate = unreliableTaxpayer.RelevanceDate,
                            DocumentDate = unreliableTaxpayer.DocumentDate,
                            IdListType = unreliableTaxpayer.IdListType,
                            IdTypeDocument = unreliableTaxpayer.IdTypeDocument,
                            Note = unreliableTaxpayer.Note,
                            DocumentNumber = unreliableTaxpayer.DocumentNumber,
                            BinCompany = unreliableTaxpayer.BinCompany
                        })
                    .On(x => new {x.BinCompany, x.IdListType}
                    ).RunAsync();
            }
            catch (Exception e)
            {
                Logger.Trace($"BIN: {unreliableTaxpayer.BinCompany}; {e.Message}");
            }

            // Inserting company director information if exist
            if (unreliableTaxpayer.BinCompany != null && unreliableTaxpayer.IinHead != null)
                try
                {
                    await using var webCompanyDirectorContext = new WebCompanyDirectorContext();
                    webCompanyDirectorContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    await webCompanyDirectorContext.Upsert(new CompanyDirector
                    {
                        CompanyBin = unreliableTaxpayer.BinCompany,
                        DirectorIin = unreliableTaxpayer.IinHead,
                        DataSource = 1,
                        RelevanceDate = DateTime.Now
                    }).On(x => x.CompanyBin).RunAsync();
                }
                catch (Exception e)
                {
                    Logger.Trace($"BIN: {unreliableTaxpayer.BinCompany}; {e.Message}");
                }
            
            lock (_forLock)
                Logger.Trace(--_count);
        }

        private async Task MigrateAsync()
        {
            await using var parsedUnreliableTaxpayerContext = new ParsedUnreliableTaxpayerContext();
            parsedUnreliableTaxpayerContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var taxpayers = parsedUnreliableTaxpayerContext.UnreliableTaxpayerDtos;
            var tasks = new List<Task>();
            foreach (var taxpayer in taxpayers)
            {
                tasks.Add(Insert(taxpayer));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}