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
    public class ErgTenderMigrationService : MigrationService
    {
        private int _total=0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _measures = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _methods = new Dictionary<string, long?>();
        private readonly Dictionary<string, long?> _documentationTypes = new Dictionary<string, long?>();

        public ErgTenderMigrationService(int numOfThreads = 5)
        {
            NumOfThreads = numOfThreads;
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            
            await MigrateReferences();
            
            var tasks = new List<Task>();
            for(var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            
            await Task.WhenAll(tasks);
            
            await using var parsed = new ParsedErgTenderContext();
            await using var webTenderContext = new WebTenderContext();
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.announcements_search;");
            await webTenderContext.Database.ExecuteSqlRawAsync("refresh materialized view adata_tender.lots_search;");
            await parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.erg_tender, avroradata.erg_tender_positions, avroradata.erg_tender_docs restart identity cascade;");
        }


        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var parsed = new ParsedErgTenderContext();
            var ergTenders = parsed.ErgTenderes
                .AsNoTracking()
                .Include(x => x.ErgTenderPositionses)
                .Include(x=>x.ErgTenderDocses);
            foreach (var ergTender in ergTenders.Where(x=>x.Id%NumOfThreads == threadNum))
            {
               await Insert(ergTender);
               
            }
            
        }
        
        private async Task Insert(ErgTenderDto.ErgTender ergTender)
        {
            var announcement = DtoToWebAnnoucement(ergTender);
            try
            {
                await using var web = new WebTenderContext();
                web.ChangeTracker.AutoDetectChangesEnabled = false;
                var found = web.AdataAnnouncements.Select(x => new {x.Id, x.SourceNumber, x.SourceId})
                    .FirstOrDefault(x =>
                        x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                if (found != null)
                {
                    await web.AdataAnnouncements.Upsert(announcement).On(x => new {x.SourceNumber, x.SourceId})
                        .UpdateIf((x, y) => x.StatusId != y.StatusId || x.MethodId != y.MethodId).RunAsync();

                    foreach (var lot in announcement.Lots)
                    {
                        lot.AnnouncementId = found.Id;
                        var foundLot = web.AdataLots.Select(x => new {x.Id, x.SourceId, x.SourceNumber})
                            .FirstOrDefault(x => x.SourceNumber == lot.SourceNumber && x.SourceId == lot.SourceId);
                        if (foundLot != null)
                        {
                            await web.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .UpdateIf((x, y) =>
                                    x.StatusId != y.StatusId || x.Characteristics != y.Characteristics ||
                                    x.MethodId != y.MethodId || x.MeasureId != y.MeasureId ||
                                    x.SupplyLocation != y.SupplyLocation).RunAsync();
                        }
                        else
                        {
                            await web.AdataLots.AddAsync(lot);
                            await web.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    await web.AdataAnnouncements.AddAsync(announcement);
                    await web.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
               Logger.Error(e,"In the Insert method");
            }
            
            lock (_lock)
                Logger.Trace($"Left {--_total}");

        }

        private async Task MigrateReferences()
        {
            await using var parsed = new ParsedErgTenderContext();
            await using var web = new WebTenderContext();

            var units = parsed.ErgTenderPositionses.Select(x => new Measure {Name = x.Unit}).Distinct()
                .Where(x => x.Name != null);
            await web.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();

            var truCodes = parsed.ErgTenderPositionses.Select(x => new TruCode {Code = x.TruCode, Name = x.ItemName}).Distinct()
                .Where(x=>x.Name != null && x.Code != null);
            await web.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();


            var statuses = parsed.ErgTenderes.Select(x => new Status {Name = x.Status}).Distinct()
                .Where(x => x.Name != null);
            await web.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();

            var methods = parsed.ErgTenderes.Select(x => new Method {Name = x.Purchase}).Distinct()
                .Where(x => x.Name != null);
            await web.Methods.UpsertRange(methods).On(x => x.Name).NoUpdate().RunAsync();

            var docTypes = parsed.ErgTenderDocses.Select(x => new DocumentationType {Name = x.Name}).Distinct()
                .Where(x => x.Name != null);
            await web.DocumentationTypes.UpsertRange(docTypes).On(x => x.Name).NoUpdate().RunAsync();
           
            foreach (var measure in web.Measures)
                _measures.TryAdd(measure.Name,measure.Id);
            
            foreach (var statuse in web.Statuses)
                _statuses.TryAdd(statuse.Name,statuse.Id);

            foreach (var method in web.Methods)
                _methods.TryAdd(method.Name,method.Id);
            
            await foreach (var type in web.DocumentationTypes)
                _documentationTypes.TryAdd(type.Name,type.Id);
            
        }

        private AdataAnnouncement DtoToWebAnnoucement(ErgTenderDto.ErgTender ergTender)
        {
            var announcement = new AdataAnnouncement
            {
                SourceNumber = ergTender.Num,
                ApplicationFinishDate = ergTender.EndDate,
                ApplicationStartDate = ergTender.PublishDate,
                PublishDate = ergTender.PublishDate,
                Title = ergTender.MainCategory,
                CustomerBin = ergTender.Bin,
                SourceId = 11,
                EmailAddress = ergTender.ContactEmail,
                PhoneNumber = ergTender.ContactTel,
                SourceLink =  $"https://torgi.erg.kz/_layouts/Supp/#/competitions/{ergTender.ErgTenderPositionses.Select(x=>x.ConcourseId)}/offer-positions"
            };
            if (ergTender.Status != null)
            {
                if (_statuses.TryGetValue(ergTender.Status, out var temp))
                {
                    announcement.StatusId = temp;
                }
            }

            if (ergTender.Purchase != null)
            {
                if (_methods.TryGetValue(ergTender.Purchase, out var temp))
                {
                    announcement.MethodId = temp;
                }
            }
            
            announcement.Documentations = new List<AnnouncementDocumentation>();
            foreach (var ergTenderErgTenderDocse in ergTender.ErgTenderDocses)
            {
                var doc = new AnnouncementDocumentation
                {
                    AnnouncementId = ergTenderErgTenderDocse.AuctionId,
                    Location = ergTenderErgTenderDocse.DocPath,
                    SourceLink = ergTenderErgTenderDocse.DocLink,
                    RelevanceDate = ergTenderErgTenderDocse.RelevanceDate,
                    Name = ergTenderErgTenderDocse.Name,
                };

                if (ergTenderErgTenderDocse.Name != null)
                {
                    if (_documentationTypes.TryGetValue(ergTenderErgTenderDocse.Name, out var temp))
                    {
                        doc.DocumentationTypeId = temp;
                    }
                }
                
                announcement.Documentations.Add(doc);
            }
            
            announcement.Lots = new List<AdataLot>();
            foreach (var ergTenderErgTenderPositionse in ergTender.ErgTenderPositionses)
            {
                if(announcement.Lots.Exists(x=>x.AnnouncementId == ergTenderErgTenderPositionse.ConcourseId))
                    continue;
                
                var lot = new AdataLot
                {
                    AnnouncementId = ergTenderErgTenderPositionse.ConcourseId,
                    SourceNumber = ergTender.Num,
                    Title = ergTenderErgTenderPositionse.ItemName,
                    ApplicationFinishDate = ergTender.EndDate,
                    ApplicationStartDate = ergTender.PublishDate,
                    TruCode = ergTenderErgTenderPositionse.TruCode,
                    Quantity = ergTenderErgTenderPositionse.Count,
                    CustomerBin = ergTender.Bin,
                    SourceId = 11,
                    SourceLink =
                        $"https://torgi.erg.kz/_layouts/Supp/#/competitions/{ergTenderErgTenderPositionse.ConcourseId}/offer-positions"
                };
                if (ergTender.Status != null)
                {
                    if (_statuses.TryGetValue(ergTender.Status, out var temp))
                    {
                        lot.StatusId = temp;
                    }
                }

                if (ergTender.Purchase != null)
                {
                    if (_methods.TryGetValue(ergTender.Purchase, out var temp))
                    {
                        lot.MethodId = temp;
                    }
                }

                if (ergTenderErgTenderPositionse.Unit != null)
                {
                    if (_methods.TryGetValue(ergTenderErgTenderPositionse.Unit, out var temp))
                    {
                        lot.MeasureId = temp;
                    }
                }
                announcement.Lots.Add(lot);
            }

            return announcement;
        }
    }
}