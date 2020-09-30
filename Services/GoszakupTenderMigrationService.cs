﻿using System.Collections.Generic;
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
        private List<Status> webStatuses;
        private List<Method> webMethods;
        private List<DocumentationType> webDocTypes;
        private List<Measure> webMeasures;
        private List<TruCode> webTruCodes;

        public GoszakupTenderMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedGoszakupTenderContext();
            _total = parsingContext.Announcements.Count();

            NumOfThreads = numOfThreads;

            using var webTenderContext = new WebTenderContext();

            webStatuses = webTenderContext.Statuses.ToList();
            webMethods = webTenderContext.Methods.ToList();
            webDocTypes = webTenderContext.DocumentationTypes.ToList();
            webMeasures = webTenderContext.Measures.ToList();
            webTruCodes = webTenderContext.TruCodes.ToList();
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
            foreach (var dto in parsedAnnouncementGoszakupContext.Announcements.AsNoTracking()
                .Include(x => x.Lots)
                .ThenInclude(x => x.Files)
                .Include(x => x.Files)
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
            
            await using var parsedGoszakupContext = new ParsedGoszakupTenderContext();
            await parsedGoszakupContext.Database.ExecuteSqlRawAsync("truncate table avroradata.announcement_goszakup restart identity cascade");
            
            Logger.Info("Successfully truncated with cascade avroradata.announcement_goszakup table");
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
                webLot.SourceLink = webAnnouncement.SourceLink;

                await using var lotWebTenderContext = new WebTenderContext();
                lotWebTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                lotWebTenderContext.AdataLots.Upsert(webLot).On(x => new {x.SourceId, x.SourceNumber})
                    .UpdateIf((x, y) => x.SourceNumber != y.SourceNumber).Run();

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
            lock (webDocTypes)
            {
                var docTypeId = webDocTypes.FirstOrDefault(x => x.Name == announcementFile.NameRu)?.Id;
                if (docTypeId == null)
                {
                    var newWebDocType = new DocumentationType()
                    {
                        Name = announcementFile.NameRu
                    };
                    using var ctx = new WebTenderContext();
                    ctx.DocumentationTypes.Add(newWebDocType);
                    ctx.SaveChanges();
                    webDocTypes.Add(newWebDocType);
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
            lock (webDocTypes)
            {
                var docTypeId = webDocTypes.FirstOrDefault(x => x.Name == lotFile.NameRu)?.Id;
                if (docTypeId == null)
                {
                    var newWebDocType = new DocumentationType
                    {
                        Name = lotFile.NameRu
                    };
                    using var ctx = new WebTenderContext();
                    ctx.DocumentationTypes.Add(newWebDocType);
                    ctx.SaveChanges();
                    webDocTypes.Add(newWebDocType);
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
                lock (webStatuses)
                {
                    statusId = webStatuses.FirstOrDefault(x => x.Name == announcementGoszakupDto.BuyStatus)?.Id;
                    if (statusId == null)
                    {
                        var newWebStatus = new Status()
                        {
                            Name = announcementGoszakupDto.BuyStatus
                        };
                        using var ctx = new WebTenderContext();
                        ctx.Statuses.Add(newWebStatus);
                        ctx.SaveChanges();
                        webStatuses.Add(newWebStatus);
                        statusId = newWebStatus.Id;
                    }
                }

            if (announcementGoszakupDto.TradeMethod != null)
                lock (webMethods)
                {
                    methodId = webMethods.FirstOrDefault(x => x.Name == announcementGoszakupDto.TradeMethod)?.Id;
                    if (methodId == null)
                    {
                        var newWebMethod = new Method
                        {
                            Name = announcementGoszakupDto.TradeMethod
                        };
                        using var ctx = new WebTenderContext();
                        ctx.Methods.Add(newWebMethod);
                        ctx.SaveChanges();
                        webMethods.Add(newWebMethod);
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
            lock (webStatuses)
            {
                statusId = webStatuses.FirstOrDefault(x => x.Name == lotGoszakupDto.LotStatus)?.Id;
                if (statusId == null)
                {
                    var newWebStatus = new Status()
                    {
                        Name = lotGoszakupDto.LotStatus
                    };
                    using var ctx = new WebTenderContext();
                    ctx.Statuses.Add(newWebStatus);
                    ctx.SaveChanges();
                    webStatuses.Add(newWebStatus);
                    statusId = newWebStatus.Id;
                }
            }

            lock (webTruCodes)
            {
                if (webTruCodes.All(x => x.Code != lotGoszakupDto.Tru))
                {
                    var newWebTruCode = new TruCode()
                    {
                        Code = lotGoszakupDto.Tru
                    };
                    using var ctx = new WebTenderContext();
                    ctx.TruCodes.Add(newWebTruCode);
                    ctx.SaveChanges();
                    webTruCodes.Add(newWebTruCode);
                }
            }

            lock (webMeasures)
            {
                measureId = webMeasures.FirstOrDefault(x => x.Name == lotGoszakupDto.Units)?.Id;
                if (measureId == null)
                {
                    var newWebMeasure = new Measure
                    {
                        Name = lotGoszakupDto.Units
                    };
                    using var ctx = new WebTenderContext();
                    ctx.Measures.Add(newWebMeasure);
                    ctx.SaveChanges();
                    webMeasures.Add(newWebMeasure);
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
                TruCode = lotGoszakupDto.Tru,
                Characteristics = lotGoszakupDto.DescriptionRu,
                TotalAmount = (double) lotGoszakupDto.Amount,
                MeasureId = measureId,
            };
            if (lotGoszakupDto.Count != null) lot.Quantity = (double) lotGoszakupDto.Count;
            if (lotGoszakupDto.Amount != null) lot.TotalAmount = (double) lotGoszakupDto.Amount;
            return lot;
        }
    }
}