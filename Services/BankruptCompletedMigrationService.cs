using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class BankruptCompletedMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _total;
        public BankruptCompletedMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
            using var parsedBankruptsAtStageContext = new ParsedBankruptAtStageContext(); 
            _total = parsedBankruptsAtStageContext.BankruptAtStageDtos.Count();
            _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateReferences();
            Logger.Warn(NumOfThreads);
            Logger.Info("Start");
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
                tasks.Add(MigrateAsync(i));
            await Task.WhenAll(tasks);
            await using var parsedBankruptCompletedContext = new ParsedBankruptCompletedContext();
            await using var webBankruptCompletedContext = new WebBankruptCompletedContext();
            var lastDate = await  parsedBankruptCompletedContext.BankruptCompletedDtos.MinAsync(x => x.DateOfRelevance);
            webBankruptCompletedContext.BankruptCompleteds.RemoveRange(webBankruptCompletedContext.BankruptCompleteds.Where(x=>x.DateOfRelevance<lastDate));
            await webBankruptCompletedContext.SaveChangesAsync();
        }
        private async Task MigrateAsync(int threadNum)
        {
            await using var webBankruptCompletedContext = new WebBankruptCompletedContext();
            await using var parsedBankruptCompletedContext = new ParsedBankruptCompletedContext();
            var bankruptDtos = from dto in parsedBankruptCompletedContext.BankruptCompletedDtos
                where dto.Id % NumOfThreads == threadNum
                select new BankruptCompleted
                {
                    DateDecision = dto.DateDecision,
                    DateEntry = dto.DateEntry,
                    DateDecisionEnd = dto.DateDecisionEnd,
                    DateEntryEnd = dto.DateEntryEnd,
                    DateOfRelevance = dto.DateOfRelevance,
                    BiinCompanies = long.Parse(dto.Bin)
                };;
            // foreach (var bankruptCompletedDto in bankruptDtos)
            // {
            //     var bankrupt = DtoToEntity(bankruptCompletedDto);
            await webBankruptCompletedContext.BankruptCompleteds.UpsertRange(bankruptDtos).On(x => x.BiinCompanies).RunAsync();
            // lock (_forLock)
            // {
                // Logger.Trace(_total--);
                // }
            // }
        }
        private async Task MigrateReferences()
        {
            await using var webBankruptCompletedContext = new WebBankruptCompletedContext();
            await using var parsedBankruptCompletedContext = new ParsedBankruptCompletedContext();
            var addresses = parsedBankruptCompletedContext.BankruptCompletedDtos.Select(x => x.Address).Distinct();
            foreach (var address in addresses)
            {
                await webBankruptCompletedContext.BankruptCAddresses.Upsert(new BankruptCAddress
                {
                    Name = address
                }).On(x => x.Name).RunAsync();
            }

            var regions = parsedBankruptCompletedContext.BankruptCompletedDtos.Select(x => x.Region).Distinct();
            foreach (var region in regions)
            {
                await webBankruptCompletedContext.RegionCs.Upsert(new RegionC
                {
                    Name = region
                }).On(x => x.Name).RunAsync();
            }

            var services = parsedBankruptCompletedContext.BankruptCompletedDtos.Select(x => x.TypeOfService).Distinct();
            foreach (var service in services)
            {
                await webBankruptCompletedContext.TypeOfServiceCs.Upsert(new TypeOfServiceC
                {
                    Name = service
                }).On(x => x.Name).RunAsync();
            }
        }
        private BankruptCompleted DtoToEntity(BankruptCompletedDto bankruptCompletedDto)
        {
            var bankruptAtStage = new BankruptCompleted
            {
                DateDecision = bankruptCompletedDto.DateDecision,
                DateEntry = bankruptCompletedDto.DateEntry,
                DateDecisionEnd = bankruptCompletedDto.DateDecisionEnd,
                DateEntryEnd = bankruptCompletedDto.DateEntryEnd,
                DateOfRelevance = bankruptCompletedDto.DateOfRelevance,
                BiinCompanies = long.Parse(bankruptCompletedDto.Bin)
            };
            return bankruptAtStage;
        }
    }
}