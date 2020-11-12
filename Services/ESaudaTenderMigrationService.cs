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
    public class ESaudaTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly Dictionary<string, long?> _statuses = new Dictionary<string, long?>();

        public ESaudaTenderMigrationService(int numOfThreads = 5)
        {
            NumOfThreads = numOfThreads;
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            await MigrateReferences();
            await Migrate();
            Logger.Info("End of migration");

            await using var parsedESaudaTenderContext = new ParsedESaudaTenderContext();
            await parsedESaudaTenderContext.Database.ExecuteSqlRawAsync("truncate table avroradata.esauda_tender restart identity");
        }

        private async Task MigrateReferences()
        {
            await using var web = new WebTenderContext();
            await using var parsed = new ParsedESaudaTenderContext();
            _total = await parsed.EsaudaTenderDtos.CountAsync();

            var statuses = parsed.EsaudaTenderDtos.Select(x => new Status {Name = x.BuyStatus}).Distinct()
                .Where(x => x.Name != null);
            await web.Statuses.UpsertRange(statuses).On(x => x.Name).NoUpdate().RunAsync();
            
            foreach (var dict in web.Statuses)
                _statuses.Add(dict.Name, dict.Id);
        }
        
        private async Task Migrate()
        {
            Logger.Info("Started Thread");
            await using var parsed = new ParsedESaudaTenderContext();
            var dtos = await parsed.EsaudaTenderDtos.ToListAsync();
            
            var tasks = new List<Task>();
            foreach (var dto in dtos)
            {
                tasks.Add(Insert(dto));
                if (tasks.Count <= NumOfThreads) continue;
                await Task.WhenAny(tasks);
                tasks.RemoveAll(x => x.IsCompleted);
            }

            await Task.WhenAll(tasks);
        }

        private async Task Insert(EsaudaTenderDto dto)
        {
            var announcement = DtoToWebAnnouncement(dto);
            try
            {
                await using var web = new WebTenderContext();
                web.ChangeTracker.AutoDetectChangesEnabled = false;
                await web.AdataAnnouncements.Upsert(announcement).On(x=> new {x.SourceNumber, x.SourceId}).RunAsync();
                await web.SaveChangesAsync();
            }
            
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                Console.WriteLine(e.InnerException);
            }
            
            lock (_lock)
                Logger.Trace($"Left {--_total}");
        }

        private AdataAnnouncement DtoToWebAnnouncement(EsaudaTenderDto dto)
        {
            var announcement = new AdataAnnouncement
            {
                ApplicationFinishDate = dto.EndDatetime,
                ApplicationStartDate = dto.StartDate,
                CustomerBin = dto.Bin,
                SourceLink = dto.Path,
                MethodId = 9,
                RelevanceDate = DateTime.Now,
                SourceId = 12,
                Title = dto.TitleRu,
                SourceNumber = dto.AuctionId.ToString()
            };
            
            if (dto.BuyStatus != null)
            {
                if (_statuses.TryGetValue(dto.BuyStatus, out var temp))
                {
                    announcement.StatusId = temp;
                }    
            }
            return announcement;
        }
    }
}