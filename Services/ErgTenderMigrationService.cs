using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
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

        public ErgTenderMigrationService(int numOfThreads = 5)
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
            
            await using var parsed = new ParsedErgTenderContext();
            await using var web = new WebTenderContext();
            var tasks = new List<Task>();

            await MigrateReferences();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            await web.SaveChangesAsync();
        }
        

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started Thread");
            await using var parsed = new ParsedErgTenderContext();
            await using var web = new WebTenderContext();
            var list = new List<AdataAnnouncement>();
            foreach (var ergTenderDto in parsed.ErgBiddingConcourse.Where(x => x.Id % NumOfThreads == threadNum))
            {
                
                var announcement = new AdataAnnouncement
                {
                    SourceNumber = ergTenderDto.Num,
                    ApplicationFinishDate = ergTenderDto.EndDate,
                    ApplicationStartDate = ergTenderDto.PublishDate,
                    PublishDate = ergTenderDto.PublishDate,
                    Title = ergTenderDto.MainCategory,
                    CustomerBin = ergTenderDto.Bin,
                    SourceId = 11
                };
                if (ergTenderDto.Status != null)
                {
                    if (_statuses.TryGetValue(ergTenderDto.Status, out var temp))
                    {
                        announcement.StatusId = temp;
                    }
                }

                if (ergTenderDto.Purchase != null)
                {
                    if (_methods.TryGetValue(ergTenderDto.Purchase, out var temp))
                    {
                        announcement.MethodId = temp;
                    }
                }

                announcement.Documentations = new List<AnnouncementDocumentation>();
                foreach (var ergBiddingConcoursePositionse in ergTenderDto.ErgBiddingConcoursePositions)
                {
                    var lot = new AdataLot
                    {
                        SourceNumber = ergTenderDto.Num,
                        Title = ergBiddingConcoursePositionse.ItemName,
                        ApplicationFinishDate = ergTenderDto.EndDate,
                        ApplicationStartDate = ergTenderDto.PublishDate,
                        TruCode = ergBiddingConcoursePositionse.TruCode,
                        Quantity = ergBiddingConcoursePositionse.Count,
                        CustomerBin = ergTenderDto.Bin,
                        SourceId = 11,
                        SourceLink =
                            $"https://torgi.erg.kz/_layouts/Supp/#/competitions/{ergBiddingConcoursePositionse.ConcourseId}/offer-positions"
                    };
                    if (ergTenderDto.Status != null)
                    {
                        if (_statuses.TryGetValue(ergTenderDto.Status, out var temp))
                        {
                            lot.StatusId = temp;
                        }
                    }

                    if (ergTenderDto.Purchase != null)
                    {
                        if (_methods.TryGetValue(ergTenderDto.Purchase, out var temp))
                        {
                            lot.MethodId = temp;
                        }
                    }

                    if (ergBiddingConcoursePositionse.Unit != null)
                    {
                        if (_methods.TryGetValue(ergBiddingConcoursePositionse.Unit, out var temp))
                        {
                            lot.MeasureId = temp;
                        }
                    }

                    announcement.Lots.Add(lot);
                }
                list.Add(announcement);
            }

            await web.AdataAnnouncements.UpsertRange(list).On(x => new {x.CustomerBin, x.SourceNumber}).RunAsync();
            await web.SaveChangesAsync();
        }

        private async Task MigrateReferences()
        {
            await using var parsed = new ParsedErgTenderContext();
            await using var web = new WebTenderContext();

            var units = parsed.ErgBiddingConcoursePositions.Select(x => new Measure {Name = x.Unit}).Distinct()
                .Where(x => x.Name != null);
            await web.Measures.UpsertRange(units).On(x => x.Name).NoUpdate().RunAsync();

            var truCodes = parsed.ErgBiddingConcoursePositions.Select(x => new TruCode {Code = x.TruCode, Name = x.ItemName}).Distinct()
                .Where(x=>x.Name != null && x.Code != null);
            await web.TruCodes.UpsertRange(truCodes).On(x => x.Code).NoUpdate().RunAsync();


            var statuses = parsed.ErgBiddingConcourse.Select(x => new Status {Name = x.Status}).Distinct()
                .Where(x => x.Name != null);
            await web.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();

            foreach (var measure in web.Measures)
                _measures.Add(measure.Name,measure.Id);   
            foreach (var statuse in web.Statuses)
                _statuses.Add(statuse.Name,statuse.Id);
            foreach (var method in web.Methods)
                _methods.Add(method.Name,method.Id);
        }
    }
}