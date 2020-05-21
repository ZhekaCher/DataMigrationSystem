using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using NLog;
using System.Linq;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class BankruptAtStageMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _total;

        public BankruptAtStageMigrationService(int numOfThreads = 20)
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
            
        }

        private async Task MigrateAsync(int threadNum)
        {
            await using var webBankruptAtStageContext = new WebBankruptAtStageContext();
            await using var parsedBankruptAtStageContext = new ParsedBankruptAtStageContext();
            var bankruptDtos = from dto in parsedBankruptAtStageContext.BankruptAtStageDtos
                where dto.Id % NumOfThreads == threadNum
                select dto;
            foreach (var bankruptAtStageDto in bankruptDtos)
            {
                var bankrupt = DtoToEntity(bankruptAtStageDto);
                await webBankruptAtStageContext.BankruptAtStages.Upsert(bankrupt).On(x => x.BiinCompanies).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_total--);
                }
            }
        }

        private async Task MigrateReferences()
        {
            await using var webBankruptAtStageContext = new WebBankruptAtStageContext();
            await using var parsedBankruptAtStageContext = new ParsedBankruptAtStageContext();
            var addresses = parsedBankruptAtStageContext.BankruptAtStageDtos.Select(x => x.Address).Distinct();
            foreach (var address in addresses)
            {
                await webBankruptAtStageContext.BankruptSAddresses.Upsert(new BankruptSAddress
                    {
                        Name = address
                    }).On(x => x.Name).RunAsync();
            }

            var regions = parsedBankruptAtStageContext.BankruptAtStageDtos.Select(x => x.Region).Distinct();
            foreach (var region in regions)
            {
                await webBankruptAtStageContext.RegionSes.Upsert(new RegionS
                {
                    Name = region
                }).On(x => x.Name).RunAsync();
            }

            var services = parsedBankruptAtStageContext.BankruptAtStageDtos.Select(x => x.TypeOfService).Distinct();
            foreach (var service in services)
            {
                await webBankruptAtStageContext.TypeOfServiceSes.Upsert(new TypeOfServiceS
                {
                    Name = service
                }).On(x => x.Name).RunAsync();
            }
        }

        private BankruptAtStage DtoToEntity(BankruptAtStageDto bankruptAtStageDto)
        {
            var bankruptAtStage = new BankruptAtStage
            {
                RelevanceDate = bankruptAtStageDto.DateOfRelevance,
                DateOfEntryIntoForce = bankruptAtStageDto.DateOfEntryIntoForce,
                DateOfCourtDecision = bankruptAtStageDto.DateOfCourtDecision,
                BiinCompanies = long.Parse(bankruptAtStageDto.Bin)
            };
            return bankruptAtStage;
        }
    }
}