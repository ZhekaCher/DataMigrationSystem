using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class GoszakupTenderDocumentationMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();
        private List<DocumentationType> docTypes;

        public GoszakupTenderDocumentationMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedGoszakupTenderContext();
            _total = parsingContext.Documents.Count();
            using var tenderContext = new WebTenderContext();
            docTypes = tenderContext.DocumentationTypes.ToList();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();

            await using var parsedAnnouncementGoszakupContext = new ParsedGoszakupTenderContext();
            parsedAnnouncementGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;

            var docs = parsedAnnouncementGoszakupContext.Documents.AsNoTracking();
            foreach (var tenderDocumentGoszakupDto in docs)
            {
                tasks.Add(Proceed(tenderDocumentGoszakupDto));
                if (tasks.Count < NumOfThreads) continue;
                await Task.WhenAny(tasks);

                tasks.Where(x => x.IsFaulted).ToList().ForEach(x =>
                    Logger.Warn(x.Exception.InnerException == null
                        ? x.Exception.Message
                        : x.Exception.InnerException.Message));

                tasks.RemoveAll(x => x.IsCompleted);
            }

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            
            var parsedGoszakupContext = new ParsedGoszakupTenderContext();
            await parsedGoszakupContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.tender_document_goszakup restart identity cascade;");

            Logger.Info("Successfully truncated with cascade avroradata.contract_goszakup table");
        }

        private async Task Proceed(TenderDocumentGoszakupDto dto)
        {
            var ctx = new WebTenderContext();
            lock (_lock)
            {
                if (docTypes.All(x => x.Name != dto.Type))
                {
                    var newDocType = new DocumentationType()
                    {
                        Name = dto.Type
                    };
                    ctx.DocumentationTypes.Add(newDocType);
                    ctx.SaveChanges();
                    docTypes.Add(newDocType);
                }
            }

            switch (dto.Identity)
            {
                case "Announcement":
                    var annoId = ctx.AdataAnnouncements.FirstOrDefault(x => x.SourceNumber == dto.Number).Id;
                    var annoDoc = new AnnouncementDocumentation()
                    {
                        Name = dto.Title,
                        DocumentationTypeId = docTypes.FirstOrDefault(x => x.Name == dto.Type).Id,
                        SourceLink = dto.Link,
                        RelevanceDate = DateTime.Now,
                        AnnouncementId = annoId
                    };
                    var existingAnnoDoc = ctx.AnnouncementDocumentations.FirstOrDefault(x =>
                        x.AnnouncementId == annoId && x.SourceLink == annoDoc.SourceLink &&
                        x.DocumentationTypeId == annoDoc.DocumentationTypeId && x.Name == annoDoc.Name);
                    if (existingAnnoDoc == null)
                    {
                        await ctx.AnnouncementDocumentations.AddAsync(annoDoc);
                        await ctx.SaveChangesAsync();
                    }

                    break;
                case "Lot":
                    var lotId = ctx.AdataLots.FirstOrDefault(x => x.SourceNumber == dto.Number).Id;
                    var lotDoc = new LotDocumentation()
                    {
                        Name = dto.Title,
                        DocumentationTypeId = docTypes.FirstOrDefault(x => x.Name == dto.Type).Id,
                        SourceLink = dto.Link,
                        RelevanceDate = DateTime.Now,
                        LotId = lotId
                    };
                    var existingLotDoc = ctx.LotDocumentations.FirstOrDefault(x =>
                        x.LotId == lotId && x.SourceLink == lotDoc.SourceLink &&
                        x.DocumentationTypeId == lotDoc.DocumentationTypeId && x.Name == lotDoc.Name);
                    if (existingLotDoc == null)
                    {
                        await ctx.LotDocumentations.AddAsync(lotDoc);
                        await ctx.SaveChangesAsync();
                    }

                    break;
            }

            lock (_lock)
            {
                Logger.Trace($"Left {--_total} docs to load");
            }
        }
    }
}