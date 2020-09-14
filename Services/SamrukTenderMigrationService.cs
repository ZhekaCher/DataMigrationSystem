using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed;
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

        public SamrukTenderMigrationService(int numOfThreads = 5)
        {
            NumOfThreads = numOfThreads;
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            
            await MigrateReferences();
            await Migrate();

            Logger.Info("End of migration");
            await using var parsedSamrukContext = new ParsedSamrukContext();
            await parsedSamrukContext.Database.ExecuteSqlRawAsync("truncate table avroradata.samruk_advert, avroradata.samruk_lots, avroradata.samruk_files restart identity");
        }

        private async Task Insert(SamrukAdvertDto dto)
        {
            await using var webTenderContext = new WebTenderContext();
            webTenderContext.ChangeTracker.AutoDetectChangesEnabled = false;
            var announcement = await DtoToWebAnnouncement(webTenderContext, dto);
            try
            {
                var found = webTenderContext.AdataAnnouncements.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                    .FirstOrDefault(x => x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
                {
                    await webTenderContext.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                        .RunAsync();
                    foreach (var lot in announcement.Lots)
                    {
                        lot.AnnouncementId = found.Id;
                        var foundLot = webTenderContext.AdataLots.Select(x => new{x.Id, x.SourceNumber, x.SourceId})
                            .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                        if (foundLot != null)
                        {
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .RunAsync();
                            lot.PaymentCondition.LotId = foundLot.Id;
                            await webTenderContext.PaymentConditions.Upsert(lot.PaymentCondition).On(x => x.LotId)
                                .RunAsync();
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
            var units = parsedSamrukContext.Lots.Select(x=> new Measure {Name = x.MkeiRussian}).Distinct();
            await webTenderContext.Measures.UpsertRange(units).On(x => x.Name).RunAsync();
            var truCodes = parsedSamrukContext.Lots.Select(x=> new TruCode {Code = x.TruCode, Name = x.TruDetailRussian}).Distinct();
            foreach (var truCode in truCodes)
            {
                await webTenderContext.TruCodes.UpsertRange(truCode).On(x => x.Code).RunAsync();
            }
            var documentationTypes = parsedSamrukContext.SamrukFiles.Select(x => new DocumentationType {Name = x.Category}).Distinct();
            await webTenderContext.DocumentationTypes.UpsertRange(documentationTypes).On(x => x.Name).RunAsync();
            var statuses = parsedSamrukContext.SamrukAdverts.Select(x => new Status{Name = x.AdvertStatus}).Distinct();
            await webTenderContext.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync(); 
        }
        private async Task<AdataAnnouncement> DtoToWebAnnouncement(WebTenderContext webTenderContext, SamrukAdvertDto dto)
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
                var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dto.AdvertStatus);
                if (status != null) 
                    announcement.StatusId = status.Id;
            }
            if (dto.TenderType != null)
            {
                var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dto.TenderType);
                if (method != null) 
                    announcement.MethodId = method.Id;
            }
            if (dto.TenderPriority != null)
            {
                var method = await  webTenderContext.TenderPriorities.FirstOrDefaultAsync(x => x.Name == dto.TenderPriority);
                if (method != null) 
                    announcement.TenderPriorityId = method.Id;
            }
            announcement.Documentations = new List<AnnouncementDocumentation>();
            if (dto.Documentations != null && dto.Documentations.Count>0)
            {
                foreach (var document in dto.Documentations.Select(fileDto => new AnnouncementDocumentation
                {
                    Name = fileDto.Name,
                    Location = fileDto.FilePath,
                    DocumentationTypeId = webTenderContext.DocumentationTypes.FirstOrDefault(x=>x.Name == fileDto.Category)?.Id
                }))
                {
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
                    Console.WriteLine();
                }
                lot.SourceLink = $"https://zakup.sk.kz/#/ext(popup:item/{lot.SourceNumber}/lot)";
                if (dtoLot.AdvertStatus != null)
                {
                    var status = await webTenderContext.Statuses.FirstOrDefaultAsync(x => x.Name == dtoLot.AdvertStatus);
                    if (status != null) 
                        lot.StatusId = status.Id;
                }
                if (dtoLot.TenderType != null)
                {
                    var method = await  webTenderContext.Methods.FirstOrDefaultAsync(x => x.Name == dtoLot.TenderType);
                    if (method != null) 
                        lot.MethodId = method.Id;
                }
                if (dtoLot.MkeiRussian != null)
                {
                    var measure = await webTenderContext.Measures.FirstOrDefaultAsync(x => x.Name == dtoLot.MkeiRussian);
                    if (measure != null) 
                        lot.MeasureId = measure.Id;
                }
                // if (dtoLot.TruCode != null)
                // {
                // var tru = await webTenderContext.TruCodes.FirstOrDefaultAsync(x => x.Code == dtoLot.TruCode);
                // if (tru != null)
                // lot.TruId = tru.Id;
                // }
                lot.Documentations = new List<LotDocumentation>();
                foreach (var document in dtoLot.Documentations.Select(fileDto => new LotDocumentation
                {
                    Name = fileDto.Name,
                    Location = fileDto.FilePath,
                    DocumentationTypeId = webTenderContext.DocumentationTypes.FirstOrDefault(x=>x.Name == fileDto.Category)?.Id
                }))
                {
                    lot.Documentations.Add(document);
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