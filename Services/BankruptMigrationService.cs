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
    public class BankruptMigrationService : MigrationService
    {
        public BankruptMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            using var parsedBankruptsAtStageContext = new ParsedBankruptContext();
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
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(MigrateAsync(i));
            await Task.WhenAll(tasks);
            await using var webBankruptContext = new WebBankruptContext();
            await using var parsedBankruptContext = new ParsedBankruptContext();
            var lastDate = await  parsedBankruptContext.BankruptAtStageDtos.MinAsync(x => x.DateOfRelevance);
            webBankruptContext.BankruptAtStages.RemoveRange(webBankruptContext.BankruptAtStages.Where(x=>x.RelevanceDate<lastDate));
            lastDate = await  parsedBankruptContext.BankruptCompletedDtos.MinAsync(x => x.DateOfRelevance);
            webBankruptContext.BankruptCompleteds.RemoveRange(webBankruptContext.BankruptCompleteds.Where(x=>x.RelevanceDate<lastDate));
            await webBankruptContext.SaveChangesAsync();
        }

        private async Task MigrateAsync(int threadNum)
        {
            await using var webBankruptContext = new WebBankruptContext();
            await using var parsedBankruptContext = new ParsedBankruptContext();
            var atStages = from dto in parsedBankruptContext.BankruptAtStageDtos
                where dto.Id % NumOfThreads == threadNum
                join com in parsedBankruptContext.CompanyBinDtos on long.Parse(dto.Bin) equals com.Code
                select new BankruptAtStage
                {
                    RelevanceDate = dto.DateOfRelevance,
                    DateOfEntryIntoForce = dto.DateOfEntryIntoForce,
                    DateOfCourtDecision = dto.DateOfCourtDecision,
                    BiinCompanies = long.Parse(dto.Bin)
                };
            await webBankruptContext.BankruptAtStages.UpsertRange(atStages).On(x => x.BiinCompanies).RunAsync();
            var completeds = from dto in parsedBankruptContext.BankruptCompletedDtos
                where dto.Id % NumOfThreads == threadNum
                join com in parsedBankruptContext.CompanyBinDtos on long.Parse(dto.Bin) equals com.Code
                select new BankruptCompleted
                {
                    DateDecision = dto.DateDecision,
                    DateEntry = dto.DateEntry,
                    DateDecisionEnd = dto.DateDecisionEnd,
                    DateEntryEnd = dto.DateEntryEnd,
                    RelevanceDate = dto.DateOfRelevance,
                    BiinCompanies = long.Parse(dto.Bin)
                };
            await webBankruptContext.BankruptCompleteds.UpsertRange(completeds).On(x => x.BiinCompanies).RunAsync();
        }

        private async Task MigrateReferences()
        {
            await using var webBankruptContext = new WebBankruptContext();
            await using var parsedBankruptContext = new ParsedBankruptContext();
            var addressesS = parsedBankruptContext.BankruptAtStageDtos.Select(x => x.Address).Distinct();
            foreach (var address in addressesS)
            {
                await webBankruptContext.BankruptSAddresses.Upsert(new BankruptSAddress
                {
                    Name = address
                }).On(x => x.Name).RunAsync();
            }

            var regionsS = parsedBankruptContext.BankruptAtStageDtos.Select(x => x.Region).Distinct();
            foreach (var region in regionsS)
            {
                await webBankruptContext.RegionSes.Upsert(new RegionS
                {
                    Name = region
                }).On(x => x.Name).RunAsync();
            }

            var servicesS = parsedBankruptContext.BankruptAtStageDtos.Select(x => x.TypeOfService).Distinct();
            foreach (var service in servicesS)
            {
                await webBankruptContext.TypeOfServiceSes.Upsert(new TypeOfServiceS
                {
                    Name = service
                }).On(x => x.Name).RunAsync();
            }
            var addressesC = parsedBankruptContext.BankruptCompletedDtos.Select(x => x.Address).Distinct();
            foreach (var address in addressesC)
            {
                await webBankruptContext.BankruptSAddresses.Upsert(new BankruptSAddress
                {
                    Name = address
                }).On(x => x.Name).RunAsync();
            }

            var regionsC = parsedBankruptContext.BankruptCompletedDtos.Select(x => x.Region).Distinct();
            foreach (var region in regionsC)
            {
                await webBankruptContext.RegionSes.Upsert(new RegionS
                {
                    Name = region
                }).On(x => x.Name).RunAsync();
            }

            var servicesC = parsedBankruptContext.BankruptCompletedDtos.Select(x => x.TypeOfService).Distinct();
            foreach (var service in servicesC)
            {
                await webBankruptContext.TypeOfServiceSes.Upsert(new TypeOfServiceS
                {
                    Name = service
                }).On(x => x.Name).RunAsync();
            }
        }
    }
}