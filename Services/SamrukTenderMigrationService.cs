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
    public class SamrukTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _priorities = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _documentationTypes = new Dictionary<string, long?>();
        
        public SamrukTenderMigrationService(int numOfThreads = 5)
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
            // await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            // await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");
            
            await using var parsedSamrukContext = new ParsedSamrukContext();
            await parsedSamrukContext.Database.ExecuteSqlRawAsync("truncate table avroradata.samruk_advert, avroradata.samruk_lots, avroradata.samruk_files restart identity");
        }

        private async Task Insert(SamrukAdvertDto dto)
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
                    foreach (var lot in announcement.Lots)
                    {
                        lot.AnnouncementId = found.Id;
                        var foundLot = webTenderContext.AdataLots.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                            .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                        if (foundLot != null)
                        {
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .UpdateIf((x, y)=> x.StatusId != y.StatusId || x.Characteristics != y.Characteristics || x.MethodId != y.MethodId || x.MeasureId != y.MeasureId || x.SupplyLocation != y.SupplyLocation).RunAsync();
                            lot.PaymentCondition.LotId = foundLot.Id;
                            await webTenderContext.PaymentConditions.Upsert(lot.PaymentCondition).On(x => x.LotId)
                                .UpdateIf((x, y) => x.InterimPayment != y.InterimPayment || x.FinalPayment != y.FinalPayment || x.PrepayPayment != y.PrepayPayment).RunAsync();
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
            await using var parsedSamrukContext = new ParsedSamrukContext();
            var samrukAdvertDtos = parsedSamrukContext.SamrukAdverts
                .AsNoTracking()
                .Include(x=>x.Lots)
                .ThenInclude(x=> x.Documentations)
                .Include(x=>x.Documentations);
            var tasks = new List<Task>();
            foreach (var dto in samrukAdvertDtos)
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
            await using var parsedSamrukContext = new ParsedSamrukContext();
            _total = await parsedSamrukContext.SamrukAdverts.CountAsync();
            var units = parsedSamrukContext.Lots.Select(x=> new Measure {Name = x.MkeiRussian}).Distinct().Where(x=>x.Name!=null);;
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();
            var truCodes = parsedSamrukContext.Lots.Select(x=> new TruCode {Code = x.TruCode, Name = x.TruDetailRussian}).Distinct().Where(x=>x.Name!=null);;
            await webTenderContext.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();
            var documentationTypes = parsedSamrukContext.SamrukFiles.Select(x => new DocumentationType {Name = x.Category}).Distinct().Where(x=>x.Name!=null);;
            await webTenderContext.DocumentationTypes.UpsertRange(documentationTypes).On(x => x.Name).NoUpdate().RunAsync();
            var statuses = parsedSamrukContext.SamrukAdverts.Select(x => new Status{Name = x.AdvertStatus}).Distinct().Where(x=>x.Name!=null);;
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync(); 
            foreach (var dict in webTenderContext.Measures)
                _measures.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Statuses)
                _statuses.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.Methods) 
                _methods.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.TenderPriorities)
                _priorities.Add(dict.Name, dict.Id);
            foreach (var dict in webTenderContext.DocumentationTypes)
                _documentationTypes.Add(dict.Name, dict.Id);
        }
        private AdataAnnouncement DtoToWebAnnouncement(SamrukAdvertDto dto)
        {
            var announcement = new AdataAnnouncement
            {
                SourceNumber =  dto.AdvertId.ToString(),
                Title =  dto.NameRussian,
                ApplicationStartDate =  dto.AcceptanceBeginDatetime,
                ApplicationFinishDate = dto.AcceptanceEndDatetime,
                CustomerBin = long.Parse(dto.CustomerBin),
                LotsAmount =  dto.SumTruNoNds ?? 0,
                LotsQuantity = dto.Lots.Count,
                SourceId = 1,
                EmailAddress = dto.Email,
                PhoneNumber = dto.Phone,
                FlagPrequalification = dto.FlagPrequalification,
                PublishDate = dto.AcceptanceBeginDatetime
            };
            announcement.SourceLink = $"https://zakup.sk.kz/#/ext(popup:item/{announcement.SourceNumber}/advert)";
            if (dto.AdvertStatus != null)
            {
                if (_statuses.TryGetValue(dto.AdvertStatus, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }
            if (dto.TenderType != null)
            {
                if (_methods.TryGetValue(dto.TenderType, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }
            if (dto.TenderPriority != null)
            {
                if (_priorities.TryGetValue(dto.TenderPriority, out var temp))
                {
                    announcement.TenderPriorityId = temp;
                }
            }
            announcement.Documentations = new List<AnnouncementDocumentation>();
            if (dto.Documentations != null && dto.Documentations.Count>0)
            {
                foreach (var documentDto in dto.Documentations)
                {
                    var document  = new AnnouncementDocumentation
                    {
                        Name = documentDto.Name,
                        Location = documentDto.FilePath,
                    };
                    if (documentDto.Category != null)
                    {
                        if (_documentationTypes.TryGetValue(documentDto.Category, out var temp))
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
                    SourceNumber = dtoLot.LotId.ToString(),
                    Title = dtoLot.NameRussian,
                    SourceId = 1,
                    ApplicationStartDate = dtoLot.AcceptanceBeginDatetime,
                    ApplicationFinishDate = dtoLot.AcceptanceEndDatetime,
                    CustomerBin = long.Parse(dtoLot.CustomerBin),
                    SupplyLocation = dtoLot.DeliveryLocation,
                    TenderLocation = dtoLot.TenderLocation, 
                    Characteristics = dtoLot.AdditionalCharacteristics,
                    TotalAmount = dtoLot.SumTruNoNds ?? 0,
                    UnitPrice = dtoLot.Price ?? 0,
                    FlagPrequalification = dtoLot.FlagPrequalification,
                    Terms = dtoLot.DeliveryTime,
                    TruCode = dtoLot.TruCode
                };
                try
                {    
                    lot.Quantity = double.Parse(dtoLot.Count, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    //ignore
                }
                lot.SourceLink = $"https://zakup.sk.kz/#/ext(popup:item/{lot.SourceNumber}/lot)";
                if (dtoLot.AdvertStatus != null)
                {
                    if (_statuses.TryGetValue(dtoLot.AdvertStatus, out var temp))
                    {
                        lot.StatusId = temp;
                    }
                }
                if (dtoLot.TenderType != null)
                {
                    if (_methods.TryGetValue(dtoLot.TenderType, out var temp))
                    {
                        lot.MethodId = temp;
                    }
                }
                if (dtoLot.MkeiRussian != null)
                {
                    if (_measures.TryGetValue(dtoLot.MkeiRussian, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }
                // if (dtoLot.TruCode != null)
                // {
                // var tru = await webTenderContext.TruCodes.FirstOrDefaultAsync(x => x.Code == dtoLot.TruCode);
                // if (tru != null)
                // lot.TruId = tru.Id;
                // }
                lot.Documentations = new List<LotDocumentation>();
                if (dtoLot.Documentations != null && dtoLot.Documentations.Count > 0)
                {
                    foreach (var documentDto in dtoLot.Documentations)
                    {
                        var document = new LotDocumentation
                        {
                            Name = documentDto.Name,
                            Location = documentDto.FilePath,
                        };
                        if (documentDto.Category != null)
                        {
                            if (_documentationTypes.TryGetValue(documentDto.Category, out var temp))
                            {
                                document.DocumentationTypeId = temp;
                            }
                        }

                        lot.Documentations.Add(document);
                    }
                }

                lot.PaymentCondition = new PaymentCondition
                {
                    PrepayPayment = dtoLot.PrepayPayment,
                    FinalPayment = dtoLot.FinalPayment,
                    InterimPayment = dtoLot.InterimPayment
                };
                announcement.Lots.Add(lot);
            }
            return announcement;
        }
    }
}