using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TenderDocumentationGoszakupMigrationService : MigrationService
    {
        private int _total;

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public TenderDocumentationGoszakupMigrationService(int numOfThreads = 20)
        {
            var parsedDocs = new ParsedGoszakupContext();
            _total = parsedDocs.TenderDocumentsGoszakup.Count();
            NumOfThreads = numOfThreads;
            parsedDocs.Dispose();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
        }

        private async Task Migrate(int threadNum)
        {
            await Task.Delay(500);
            var parsedDocs = new ParsedGoszakupContext();
            var adataTenderContextTemp = new AdataTenderContext();
            adataTenderContextTemp.Dispose();
            foreach (var dto in parsedDocs.TenderDocumentsGoszakup.AsNoTracking().Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                var adataTenderContext = new AdataTenderContext();
                var docType = adataTenderContext.DocumentationTypes.FirstOrDefault(x => x.Name == dto.Type);
                if (docType == null)
                {
                    adataTenderContext.DocumentationTypes.Add(new DocumentationType
                    {
                        Name = dto.Type
                    });
                    await adataTenderContext.SaveChangesAsync();
                    docType = adataTenderContext.DocumentationTypes.FirstOrDefault(x => x.Name == dto.Type);
                }

                if (dto.Identity == "Announcement")
                {
                    var anno =
                        adataTenderContext.AdataAnnouncements.FirstOrDefault(x => x.SourceNumber == dto.Number);
                    if (anno == null) continue;
                    await adataTenderContext.Database.ExecuteSqlRawAsync("");
                    adataTenderContext.AnnouncementDocumentations.Add(new AnnouncementDocumentation()
                    {
                        AnnouncementId = anno.Id,
                        DocumentationTypeId = docType.Id,
                        Name = dto.Title,
                        SourceLink = dto.Link
                    });

                    try
                    {
                        await adataTenderContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (dto.Identity == "Lot")
                {
                    var lot =
                        adataTenderContext.AdataLots.FirstOrDefault(x => x.SourceNumber == dto.Number);
                    if (lot == null) continue;

                    adataTenderContext.LotDocumentations.Add(new LotDocumentation
                    {
                        LotId = lot.Id,
                        DocumentationTypeId = docType.Id,
                        Name = dto.Title,
                        SourceLink = dto.Link
                    });
                }

                try
                {
                    await adataTenderContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                }

                adataTenderContext.Dispose();
                Console.WriteLine(--_total + " " + Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}