using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.TradingFloor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class GoszakupTenderMigrationService : MigrationService
    {
        private int _total = 0;
        private readonly object _lock = new object();
        private readonly int _sourceId = 2;

        public GoszakupTenderMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedGoszakupContext();
            _total = parsingContext.AnnouncementGoszakupDtos.Count();
            
            NumOfThreads = numOfThreads;

            _statusesDictionary.Add(110, 51);
            _statusesDictionary.Add(120, 52);
            _statusesDictionary.Add(130, 53);
            _statusesDictionary.Add(160, 54);
            _statusesDictionary.Add(190, 20);
            _statusesDictionary.Add(210, 1);
            _statusesDictionary.Add(220, 3);
            _statusesDictionary.Add(230, 6);
            _statusesDictionary.Add(240, 4);
            _statusesDictionary.Add(250, 8);
            _statusesDictionary.Add(260, 9);
            _statusesDictionary.Add(270, 10);
            _statusesDictionary.Add(280, 7);
            _statusesDictionary.Add(285, 55);
            _statusesDictionary.Add(290, 56);
            _statusesDictionary.Add(310, 32);
            _statusesDictionary.Add(320, 33);
            _statusesDictionary.Add(330, 34);
            _statusesDictionary.Add(350, 38);
            _statusesDictionary.Add(360, 35);
            _statusesDictionary.Add(370, 39);
            _statusesDictionary.Add(410, 40);
            _statusesDictionary.Add(420, 43);
            _statusesDictionary.Add(430, 41);
            _statusesDictionary.Add(440, 44);
            _statusesDictionary.Add(510, 45);
            _statusesDictionary.Add(520, 57);
            _statusesDictionary.Add(530, 46);
            _statusesDictionary.Add(540, 47);
            _methodsDictionary.Add(1, 22);
            _methodsDictionary.Add(2, 4);
            _methodsDictionary.Add(3, 1);
            _methodsDictionary.Add(6, 12);
            _methodsDictionary.Add(7, 9);
            _methodsDictionary.Add(8, 23);
            _methodsDictionary.Add(9, 24);
            _methodsDictionary.Add(10, 25);
            _methodsDictionary.Add(11, 26);
            _methodsDictionary.Add(12, 27);
            _methodsDictionary.Add(13, 28);
            _methodsDictionary.Add(14, 29);
            _methodsDictionary.Add(16, 30);
            _methodsDictionary.Add(22, 16);
            _methodsDictionary.Add(23, 31);
            _methodsDictionary.Add(31, 32);
            _methodsDictionary.Add(32, 18);
            _methodsDictionary.Add(40, 33);
            _methodsDictionary.Add(50, 7);
            _methodsDictionary.Add(51, 34);
            _methodsDictionary.Add(52, 8);
            _methodsDictionary.Add(105, 35);
            _methodsDictionary.Add(107, 36);
            _methodsDictionary.Add(116, 3);
            _methodsDictionary.Add(117, 37);
            _methodsDictionary.Add(118, 38);
            _methodsDictionary.Add(119, 39);
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private readonly IDictionary<int?, int> _statusesDictionary = new Dictionary<int?, int>();
        private readonly IDictionary<int?, int> _methodsDictionary = new Dictionary<int?, int>();

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");
            await using var webTenderContext = new AdataTenderContext();
            await using var parsedAnnouncementGoszakupContext = new ParsedGoszakupContext();
            Console.WriteLine(DateTime.Now);
            foreach (var dto in parsedAnnouncementGoszakupContext.AnnouncementGoszakupDtos.Where(x =>
                x.Id % NumOfThreads == threadNum).Include(x => x.Lots))
            {
                var announcement = DtoToWebAnnouncement(dto);
                try
                {
                    var found = webTenderContext.AdataAnnouncements
                        .FirstOrDefault(x =>
                            x.SourceNumber == announcement.SourceNumber && x.SourceId == announcement.SourceId);
                    if (found != null)
                    {
                        await webTenderContext.AdataAnnouncements.Upsert(announcement)
                            .On(x => new {x.SourceNumber, x.SourceId})
                            .RunAsync();
                        foreach (var lot in announcement.Lots)
                        {
                            lot.AnnouncementId = found.Id;
                            await webTenderContext.AdataLots.Upsert(lot).On(x => new {x.SourceNumber, x.SourceId})
                                .RunAsync();
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
                    if (e.Message.Contains("violates foreign key"))
                    {
                        // Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
                    }
                    else
                    {
                        Logger.Error(e);
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }

            Logger.Info($"Completed thread at {_total}");
        }

        private AdataAnnouncement DtoToWebAnnouncement(AnnouncementGoszakupDto announcementGoszakupDto)
        {
            using var webTenderContext = new AdataTenderContext();
            _statusesDictionary.TryGetValue(announcementGoszakupDto.RefBuyStatusId, out var statusId);
            _methodsDictionary.TryGetValue(announcementGoszakupDto.RefTradeMethodsId, out var methodId);
            var announcement = new AdataAnnouncement
            {
                SourceNumber = announcementGoszakupDto.NumberAnno,
                Title = announcementGoszakupDto.NameRu,

                //Refactor
                StatusId = statusId != 0 ? statusId : (long?) null,
                MethodId = methodId != 0 ? methodId : (long?) null,

                //TODO(ApplicationStartDate)
                //TODO(ApplicationFinishDate)
                CustomerBin = announcementGoszakupDto.CustomerBin,
                LotsQuantity = announcementGoszakupDto.Lots.Count,
                LotsAmount = (double) announcementGoszakupDto.Lots.Sum(dto => dto.Amount),
                SourceLink = $"https://www.goszakup.gov.kz/ru/announce/index/{announcementGoszakupDto.Id}",
                RelevanceDate = announcementGoszakupDto.Relevance,
                SourceId = _sourceId
            };
            announcement.Lots = new List<AdataLot>();

            foreach (var lotGoszakupDto in announcementGoszakupDto.Lots)
                announcement.Lots.Add(DtoToWebLot(lotGoszakupDto, webTenderContext));

            return announcement;
        }

        private AdataLot DtoToWebLot(LotGoszakupDto lotGoszakupDto, AdataTenderContext webTenderContext)
        {
            _statusesDictionary.TryGetValue(lotGoszakupDto.RefLotStatusId, out var statusId);
            _statusesDictionary.TryGetValue(lotGoszakupDto.RefTradeMethodsId, out var refTradeMethodsId);
            var lot = new AdataLot
            {
                StatusId = statusId != 0 ? statusId : (long?) null,
                MethodId = refTradeMethodsId != 0 ? refTradeMethodsId : (long?) null,
                SourceId = _sourceId,
                // Application start date
                // Application finish date
                CustomerBin = lotGoszakupDto.CustomerBin,
                // Supply location
                // Tender location
                // TruId
                Characteristics = lotGoszakupDto.DescriptionRu,
                UnitPrice = (double) (lotGoszakupDto.Amount / lotGoszakupDto.Count),
                TotalAmount = (double) lotGoszakupDto.Amount,
                // Terms
                RelevanceDate = lotGoszakupDto.Relevance
            };

            if (lotGoszakupDto.Count != null) lot.Quantity = (double) lotGoszakupDto.Count;
            if (lotGoszakupDto.Amount != null) lot.TotalAmount = (double) lotGoszakupDto.Amount;
            return lot;
        }
    }
}