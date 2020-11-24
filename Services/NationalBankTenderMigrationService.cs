using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class NationalBankTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _documentationTypes = new Dictionary<string, long?>();

        public NationalBankTenderMigrationService(int numOfThreads = 5)
        {
            NumOfThreads = numOfThreads;
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");

            await MigrateReferences();
            await Migrate();

            Logger.Info("End of migration");

            await using var webTenderContext = new WebTenderContext();
            await webTenderContext.Database.ExecuteSqlRawAsync(
                "refresh materialized view adata_tender.announcements_search;");
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");

            // await using var parsedNationalBankTenderContext = new ParsedNationalBankTenderContext();
            // await parsedNationalBankTenderContext.Database.ExecuteSqlRawAsync(
            //    "truncate table avroradata.nationalbank_advert, avroradata.nationalbank_lot, avroradata.nationalbank_files restart identity");
        }

        private async Task Insert(NationalBankTenderDto dto)
        {
            var announcement = DtoToWebAnnouncement(dto);
            try
            {
                await using var webTenderContext = new WebTenderContext();
                webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
                var found = webTenderContext.AdataAnnouncements.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                    .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
                {
                    await webTenderContext.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                        .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.LotsQuantity != y.LotsQuantity || x.MethodId != y.MethodId || x.TenderPriorityId != y.TenderPriorityId).RunAsync();
                    
                    // webTenderContext.AnnouncementDocumentations.UpsertRange(dto.Documentations.Select(x=>
                    //     new AnnouncementDocumentation
                    //     {
                    //         Name = x.DocName,
                    //         Location = x.DocFilePath,
                    //         AnnouncementId = found.Id,
                    //         SourceLink = x.DocSourceLink,
                    //         
                    //     }));
                    // await webTenderContext.SaveChangesAsync();
                    
                    foreach (var lot in announcement.Lots)
                    {
                        lot.AnnouncementId = found.Id;
                        var foundLot = webTenderContext.AdataLots.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                            .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                        if (foundLot != null)
                        {
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.Characteristics != y.Characteristics || x.MethodId != y.MethodId || x.MeasureId != y.MeasureId || x.SupplyLocation != y.SupplyLocation).RunAsync();
                           
                            
                            /*var dtoLot = dto.Lots;
                            foreach (var lotDoc in dtoLot )
                            {
                                webTenderContext.LotDocumentations.UpsertRange(lotDoc.LotDocumentations.Select(x =>
                                    new LotDocumentation
                                    {
                                        Name = x.DocName,
                                        Location = x.DocFilePath,
                                        LotId = foundLot.Id,
                                        SourceLink = x.DocSourceLink,
                                    }));
                                await webTenderContext.SaveChangesAsync();
                            }*/
                            
                        }
                        else
                        {
                            await webTenderContext.AdataLots.AddAsync(lot);
                            await webTenderContext.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    await webTenderContext.AdataAnnouncements.AddAsync(announcement);
                    await webTenderContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }

        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsedNationalBankTenderContext = new ParsedNationalBankTenderContext();
            var nationalBankTenderDtos = parsedNationalBankTenderContext.NationalBankAdvert
                .AsNoTracking()
                .Include(x => x.Lots)
                .ThenInclude(x => x.Documentations)
                .Include(x => x.Documentations);
            var tasks = new List<Task>();
            foreach (var dto in nationalBankTenderDtos)
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

        private async Task MigrateReferences()
        {
            await using var webTenderContext = new WebTenderContext();
            await using var parsedNationalBankTenderContext = new ParsedNationalBankTenderContext();
            _total = await parsedNationalBankTenderContext.NationalBankAdvert.CountAsync();
            var units = parsedNationalBankTenderContext.Lots.Select(x => new Measure {Name = x.UnitOfMeasure})
                .Distinct()
                .Where(x => x.Name != null);
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();

            var truCodes = parsedNationalBankTenderContext.Lots
                .Select(x => new TruCode {Code = x.LotTru, Name = x.LotNameRu}).Distinct()
                .Where(x => x.Name != null && x.Code != null);
            await webTenderContext.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();

            var documentationTypes = parsedNationalBankTenderContext.TenderFiles
                .Select(x => new DocumentationType {Name = x.DocCategory}).Distinct().Where(x => x.Name != null);
            await webTenderContext.DocumentationTypes.UpsertRange(documentationTypes).On(x => x.Name).NoUpdate()
                .RunAsync();

            var statuses = parsedNationalBankTenderContext.NationalBankAdvert
                .Select(x => new Status {Name = x.AdvertStatus}).Distinct()
                .Where(x => x.Name != null);
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();

            foreach (var dict in webTenderContext.Measures)
                _measures.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Statuses)
                _statuses.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Methods)
                _methods.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.DocumentationTypes)
                _documentationTypes.Add(dict.Name, dict.Id);
        }


        private AdataAnnouncement DtoToWebAnnouncement(NationalBankTenderDto dto)
        {
            var announcement = new AdataAnnouncement
            {
                Id = dto.Id,
                SourceNumber = dto.AdvertId,
                Title = dto.AdvertNameRu,
                ApplicationStartDate = dto.StartDate,
                ApplicationFinishDate = dto.FinishDate,
                CustomerBin = dto.CustomerBin,
                LotsAmount = dto.AdvertSumNoNds ?? 0,
                LotsQuantity = dto.LotCount,
                SourceId = 9,
                PublishDate = dto.StartDate,
                EmailAddress = dto.Email
            };
            announcement.SourceLink = dto.SourceLink;
            if (dto.AdvertStatus != null)
            {
                if (_statuses.TryGetValue(dto.AdvertStatus, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }

            if (dto.AdvertMethod != null)
            {
                if (_methods.TryGetValue(dto.AdvertMethod, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }

            announcement.Documentations = new List<AnnouncementDocumentation>();
            if (dto.Documentations != null && dto.Documentations.Count > 0)
            {
                foreach (var documentDto in dto.Documentations)
                {
                    var document = new AnnouncementDocumentation
                    {
                        Name = documentDto.DocName,
                        Location = documentDto.DocFilePath,
                        SourceLink = documentDto.DocSourceLink
                    };

                    if (documentDto.DocCategory != null)
                    {
                        if (_documentationTypes.TryGetValue(documentDto.DocCategory, out var temp))
                        {
                            document.DocumentationTypeId = temp;
                        }
                    }

                    announcement.Documentations.Add(document);
                }
            }

            announcement.Lots = new List<AdataLot>();
            foreach (var dtoLot in dto.Lots)
            {
                var lot = new AdataLot
                {
                    SourceNumber = dtoLot.LotId,
                    Title = dtoLot.LotNameRu,
                    SourceId = 9,
                    ApplicationStartDate = dto.StartDate,
                    ApplicationFinishDate = dto.FinishDate,
                    CustomerBin = dto.CustomerBin,
                    SupplyLocation = dtoLot.DeliveryAddress,
                    Characteristics = dtoLot.LotDescriptionRu,
                    TotalAmount = dtoLot.LotVolume ?? 0,
                    UnitPrice = dtoLot.PricePerUnit ?? 0,
                    Terms = dtoLot.LotPeriod,
                    TruCode = dtoLot.LotTru
                };
                try
                {
                    lot.Quantity = (double) dtoLot.LotAmount;
                }
                catch (Exception)
                {
                    //ignore
                }

                lot.SourceLink = dtoLot.SourceLink;
                if (dto.AdvertStatus != null)
                {
                    if (_statuses.TryGetValue(dto.AdvertStatus, out var temp))
                    {
                        lot.StatusId = temp;
                    }
                }

                if (dto.AdvertMethod != null)
                {
                    if (_methods.TryGetValue(dto.AdvertMethod, out var temp))
                    {
                        lot.MethodId = temp;
                    }
                }

                if (dtoLot.UnitOfMeasure != null)
                {
                    if (_measures.TryGetValue(dtoLot.UnitOfMeasure, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }

                lot.Documentations = new List<LotDocumentation>();
                if (dtoLot.Documentations != null && dtoLot.Documentations.Count > 0)
                {
                    foreach (var documentDto in dtoLot.Documentations)
                    {
                        var document = new LotDocumentation
                        {
                            Name = documentDto.DocName,
                            Location = documentDto.DocFilePath,
                            SourceLink = documentDto.DocSourceLink
                        };
                        if (documentDto.DocCategory != null)
                        {
                            if (_documentationTypes.TryGetValue(documentDto.DocCategory, out var temp))
                            {
                                document.DocumentationTypeId = temp;
                            }
                        }

                        lot.Documentations.Add(document);
                    }
                }

                announcement.Lots.Add(lot);
            }

            return announcement;
        }
    }
}