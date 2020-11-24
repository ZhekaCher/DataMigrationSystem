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

        public ErgTenderMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            
            await using var parsed = new ParsedErgTenderContext();
            await using var web = new WebTenderContext();

            await MigrateReferences();
            await Migrate();
            
            await parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.erg_tender, avroradata.erg_tender_positions restart identity cascade;");
        }


        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsed = new ParsedErgTenderContext();
            var ergTenders = parsed.ErgTenderes
                .AsNoTracking()
                .Include(x => x.ErgTenderPositionses);

            var tasks = new List<Task>();
            foreach (var ergTender in ergTenders)
            {
                tasks.Add(Insert(ergTender));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
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
                // ignore
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
           
            foreach (var measure in web.Measures)
                _measures.Add(measure.Name,measure.Id);
            
            foreach (var statuse in web.Statuses)
                _statuses.Add(statuse.Name,statuse.Id);

            foreach (var method in web.Methods)
                _methods.Add(method.Name,method.Id);
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
            announcement.Lots = new List<AdataLot>();
            foreach (var ergTenderErgTenderPositionse in ergTender.ErgTenderPositionses)
            {
                var lot = new AdataLot
                {
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