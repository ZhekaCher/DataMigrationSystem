using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GoszakupTenderMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();
        private const int SourceId = 2;
        private readonly List<Status> _webStatuses;
        private readonly List<Method> _webMethods;
        private readonly List<DocumentationType> _webDocTypes;
        private readonly List<Measure> _webMeasures;
        private readonly List<TruCode> _webTruCodes;

        public GoszakupTenderMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedGoszakupTenderContext();
            _total = parsingContext.Announcements.Count();

            NumOfThreads = numOfThreads;

            using var webTenderContext = new WebTenderContext();

            _webStatuses = webTenderContext.Statuses.ToList();
            _webMethods = webTenderContext.Methods.ToList();
            _webDocTypes = webTenderContext.DocumentationTypes.ToList();
            _webMeasures = webTenderContext.Measures.ToList();
            _webTruCodes = webTenderContext.TruCodes.ToList();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();

            await using var parsedAnnouncementGoszakupContext = new ParsedGoszakupTenderContext();
            parsedAnnouncementGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;
            foreach (var dto in parsedAnnouncementGoszakupContext.Announcements.AsNoTracking().OrderByDescending(x => x.PublishDate)
                .Include(x => x.Lots)
            )
            {
                tasks.Add(Proceed(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);

                    tasks.Where(x => x.IsFaulted).ToList().ForEach(x =>
                        Logger.Warn(x.Exception.InnerException == null
                            ? x.Exception.Message
                            : x.Exception.InnerException.Message));

                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");

            await using var webTenderContext = new WebTenderContext();
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");

            Logger.Info("Successfully refreshed materialized views");
            
            await webTenderContext.Database.ExecuteSqlRawAsync(@"
update adata_tender.announcements a
set status_id=58
where a.id in (select a.id
               from adata_tender.announcements a
                        inner join adata_tender.statuses s on a.status_id = s.id
               where combined_id < 5
                 and source_id = 2
                 and application_finish_date < now());
");

            await webTenderContext.Database.ExecuteSqlRawAsync(@"
update adata_tender.lots l
set status_id=38
where l.id in (select l.id
               from adata_tender.lots l
                        inner join adata_tender.statuses s on l.status_id = s.id
               where combined_id < 5
                 and source_id = 2
                 and application_finish_date < now());
");

            Logger.Info("Successfully refreshed old statuses");

            await using var parsedGoszakupContext = new ParsedGoszakupTenderContext();
            // await parsedGoszakupContext.Database.ExecuteSqlRawAsync("truncate table avroradata.announcement_goszakup restart identity cascade");

            // Logger.Info("Successfully truncated with cascade avroradata.announcement_goszakup table");
        }

        private async Task Proceed(AnnouncementGoszakupDto dto)
        {
            await Task.Delay(200);
            await using var annoWebTenderContext = new WebTenderContext();
            annoWebTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var webAnnouncement = DtoToWebAnnouncement(dto);
            annoWebTenderContext.AdataAnnouncements.Upsert(webAnnouncement).On(x => new {x.SourceId, x.SourceNumber})
                .UpdateIf((x, y) =>
                    x.Title != y.Title ||
                    x.StatusId != y.StatusId ||
                    x.MethodId != y.MethodId ||
                    x.CustomerBin != y.CustomerBin ||
                    x.LotsQuantity != y.LotsQuantity ||
                    x.LotsAmount != y.LotsAmount ||
                    x.SourceLink != y.SourceLink ||
                    x.EmailAddress != y.EmailAddress ||
                    x.PhoneNumber != y.PhoneNumber ||
                    x.FlagPrequalification != y.FlagPrequalification ||
                    x.TenderPriorityId != y.TenderPriorityId
                ).Run();

            // ReSharper disable once PossibleNullReferenceException
            var annoId = annoWebTenderContext.AdataAnnouncements.Select(x => new {x.Id, x.SourceNumber, x.SourceId})
                .FirstOrDefault(x => x.SourceNumber == webAnnouncement.SourceNumber && x.SourceId == SourceId).Id;
            //Anno docs
            // var webAnnouncementFiles = dto.Files.Select(DtoToWebAnnouncementDocumentation).ToList();
            // if (webAnnouncementFiles.Count > 0)
            // {
            //     webAnnouncementFiles.ForEach(x => x.AnnouncementId = annoId);
            //     await using var annoDocWebTenderContext = new WebTenderContext();
            //     annoDocWebTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
            //     var webAnnoExistingDocs =
            //         annoDocWebTenderContext.AnnouncementDocumentations.Where(x => x.AnnouncementId == annoId).ToList();
            //     webAnnouncementFiles = webAnnouncementFiles.Where(x => !webAnnoExistingDocs.Any(y =>
            //             x.Location == y.Location && x.Name == y.Name &&
            //             x.DocumentationTypeId == y.DocumentationTypeId))
            //         .ToList();
            //     await annoDocWebTenderContext.AnnouncementDocumentations.AddRangeAsync(webAnnouncementFiles);
            //     await annoDocWebTenderContext.SaveChangesAsync();
            // }

            // Anno lots
            foreach (var lotGoszakupDto in dto.Lots)
            {
                var webLot = DtoToWebLot(lotGoszakupDto);

                webLot.AnnouncementId = annoId;
                webLot.MethodId = webAnnouncement.MethodId;
                webLot.SourceLink = $"{webAnnouncement.SourceLink}?tab=lots";
                webLot.ApplicationStartDate = webAnnouncement.ApplicationStartDate;
                webLot.ApplicationFinishDate = webAnnouncement.ApplicationFinishDate;

                await using var lotWebTenderContext = new WebTenderContext();
                lotWebTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                lotWebTenderContext.AdataLots.Upsert(webLot).On(x => new {x.SourceId, x.SourceNumber})
                    .UpdateIf((x, y) => 
                        x.SourceNumber != y.SourceNumber ||
                        x.Characteristics != y.Characteristics ||
                        x.Terms != y. Terms ||
                        x.SupplyLocation != y.SupplyLocation ||
                        x.TruCode != y.TruCode ||
                        x.Quantity != y.Quantity ||
                        x.TotalAmount != y.TotalAmount ||
                        x.SourceNumber != y.SourceNumber ||
                        x.SourceLink != y.SourceLink || 
                        x.ApplicationFinishDate != y.ApplicationFinishDate || 
                        x.ApplicationStartDate != y.ApplicationStartDate 
                        )
                    .Run();

                //Lot docs
                // var webLotFiles = lotGoszakupDto.Files.Select(DtoToWebLotDocumentation).ToList();
                // if (webLotFiles.Count > 0)
                // {
                // var lotId = lotWebTenderContext.AdataLots.Select(x => new {x.Id, x.SourceNumber, x.SourceId})
                // .FirstOrDefault(x => x.SourceNumber == webLot.SourceNumber && x.SourceId == SourceId).Id;
                // webLotFiles.ForEach(x => x.LotId = lotId);
                // await using var lotDocWebTenderContext = new WebTenderContext();
                // lotDocWebTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                // var webLotExistingDocs =
                // lotDocWebTenderContext.LotDocumentations.Where(x => x.LotId == lotId).ToList();
                // webLotFiles = webLotFiles.Where(x => !webLotExistingDocs.Any(y =>
                // x.Location == y.Location && x.Name == y.Name &&
                // x.DocumentationTypeId == y.DocumentationTypeId))
                // .ToList();
                // await lotDocWebTenderContext.LotDocumentations.AddRangeAsync(webLotFiles);
                // await lotDocWebTenderContext.SaveChangesAsync();
                // }
            }

            lock (_lock)
                Logger.Trace($"Left {--_total} {Thread.CurrentThread.Name}");
        }

        private AnnouncementDocumentation DtoToWebAnnouncementDocumentation(
            AnnouncementFileGoszakupDto announcementFile)
        {
            var annoDoc = new AnnouncementDocumentation()
            {
                Name = announcementFile.OriginalName,
                SourceLink = announcementFile.Link
            };
            lock (_webDocTypes)
            {
                var docTypeId = _webDocTypes.FirstOrDefault(x => x.Name == announcementFile.NameRu)?.Id;
                if (docTypeId == null)
                {
                    var newWebDocType = new DocumentationType()
                    {
                        Name = announcementFile.NameRu
                    };
                    using var ctx = new WebTenderContext();
                    ctx.DocumentationTypes.Add(newWebDocType);
                    ctx.SaveChanges();
                    _webDocTypes.Add(newWebDocType);
                    docTypeId = newWebDocType.Id;
                }

                annoDoc.DocumentationTypeId = docTypeId;
            }

            return annoDoc;
        }

        private LotDocumentation DtoToWebLotDocumentation(LotFileGoszakupDto lotFile)
        {
            var lotDoc = new LotDocumentation
            {
                Name = lotFile.OriginalName,
                SourceLink = lotFile.Link
            };
            lock (_webDocTypes)
            {
                var docTypeId = _webDocTypes.FirstOrDefault(x => x.Name == lotFile.NameRu)?.Id;
                if (docTypeId == null)
                {
                    var newWebDocType = new DocumentationType
                    {
                        Name = lotFile.NameRu
                    };
                    using var ctx = new WebTenderContext();
                    ctx.DocumentationTypes.Add(newWebDocType);
                    ctx.SaveChanges();
                    _webDocTypes.Add(newWebDocType);
                    docTypeId = newWebDocType.Id;
                }

                lotDoc.DocumentationTypeId = docTypeId;
            }

            return lotDoc;
        }

        private AdataAnnouncement DtoToWebAnnouncement(AnnouncementGoszakupDto announcementGoszakupDto)
        {
            using var webTenderContext = new WebTenderContext();
            long? statusId = null;
            long? methodId = null;
            if (announcementGoszakupDto.BuyStatus != null)
                lock (_webStatuses)
                {
                    statusId = _webStatuses.FirstOrDefault(x => x.Name == announcementGoszakupDto.BuyStatus)?.Id;
                    if (statusId == null)
                    {
                        var newWebStatus = new Status()
                        {
                            Name = announcementGoszakupDto.BuyStatus
                        };
                        using var ctx = new WebTenderContext();
                        ctx.Statuses.Add(newWebStatus);
                        ctx.SaveChanges();
                        _webStatuses.Add(newWebStatus);
                        statusId = newWebStatus.Id;
                    }
                }

            if (announcementGoszakupDto.TradeMethod != null)
                lock (_webMethods)
                {
                    methodId = _webMethods.FirstOrDefault(x => x.Name == announcementGoszakupDto.TradeMethod)?.Id;
                    if (methodId == null)
                    {
                        var newWebMethod = new Method
                        {
                            Name = announcementGoszakupDto.TradeMethod
                        };
                        using var ctx = new WebTenderContext();
                        ctx.Methods.Add(newWebMethod);
                        ctx.SaveChanges();
                        _webMethods.Add(newWebMethod);
                        methodId = newWebMethod.Id;
                    }
                }

            var announcement = new AdataAnnouncement
            {
                SourceNumber = announcementGoszakupDto.NumberAnno,
                Title = announcementGoszakupDto.NameRu,

                //Refactor
                StatusId = statusId,
                MethodId = methodId,
                ApplicationStartDate = announcementGoszakupDto.StartDate,
                ApplicationFinishDate = announcementGoszakupDto.EndDate,
                CustomerBin = announcementGoszakupDto.OrganizatorBiin,
                LotsQuantity = announcementGoszakupDto.Lots.Count,
                LotsAmount = (double) announcementGoszakupDto.TotalSum,
                SourceLink =
                    $"https://www.goszakup.gov.kz/ru/announce/index/{announcementGoszakupDto.NumberAnno.Split("-")[0]}",
                PublishDate = announcementGoszakupDto.PublishDate,
                SourceId = SourceId
            };
            announcement.Lots = new List<AdataLot>();
            return announcement;
        }

        private AdataLot DtoToWebLot(LotGoszakupDto lotGoszakupDto)
        {
            long? statusId;
            long? measureId;
            lock (_webStatuses)
            {
                statusId = _webStatuses.FirstOrDefault(x => x.Name == lotGoszakupDto.LotStatus)?.Id;
                if (statusId == null)
                {
                    var newWebStatus = new Status()
                    {
                        Name = lotGoszakupDto.LotStatus
                    };
                    using var ctx = new WebTenderContext();
                    ctx.Statuses.Add(newWebStatus);
                    ctx.SaveChanges();
                    _webStatuses.Add(newWebStatus);
                    statusId = newWebStatus.Id;
                }
            }

            lock (_webTruCodes)
            {
                if (_webTruCodes.All(x => x.Code != lotGoszakupDto.TruCode))
                {
                    var newWebTruCode = new TruCode
                    {
                        Code = lotGoszakupDto.TruCode,
                        Name = lotGoszakupDto.NameRu,
                        Characteristics = lotGoszakupDto.DescriptionRu
                    };
                    using var ctx = new WebTenderContext();
                    ctx.TruCodes.Add(newWebTruCode);
                    ctx.SaveChanges();
                    _webTruCodes.Add(newWebTruCode);
                }
            }

            lock (_webMeasures)
            {
                measureId = _webMeasures.FirstOrDefault(x => x.Name == lotGoszakupDto.Units)?.Id;
                if (measureId == null)
                {
                    var newWebMeasure = new Measure
                    {
                        Name = lotGoszakupDto.Units
                    };
                    using var ctx = new WebTenderContext();
                    ctx.Measures.Add(newWebMeasure);
                    ctx.SaveChanges();
                    _webMeasures.Add(newWebMeasure);
                    measureId = newWebMeasure.Id;
                }
            }

            var lot = new AdataLot
            {
                SourceNumber = lotGoszakupDto.LotNumber,
                StatusId = statusId,
                SourceId = SourceId,
                CustomerBin = lotGoszakupDto.CustomerBin,
                Title = lotGoszakupDto.NameRu,
                SupplyLocation = lotGoszakupDto.DeliveryPlace,
                Terms = lotGoszakupDto.SupplyDateRu,
                // Tender location
                UnitPrice = (double) (lotGoszakupDto.Amount / lotGoszakupDto.Count),
                TruCode = lotGoszakupDto.TruCode,
                Characteristics = lotGoszakupDto.DescriptionRu,
                TotalAmount = (double) lotGoszakupDto.Amount,
                MeasureId = measureId
            };
            if (lotGoszakupDto.Count != null) lot.Quantity = (double) lotGoszakupDto.Count;
            if (lotGoszakupDto.Amount != null) lot.TotalAmount = (double) lotGoszakupDto.Amount;
            return lot;
        }
    }
}
