using System;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class BankruptAtStageAndCompletedMigrationService : MigrationService
    {
        private readonly WebNewBankruptContext _web;
        private readonly ParsedNewBakruptContext _parsed;
        private readonly object _forLock;
        private int _counter;

        public BankruptAtStageAndCompletedMigrationService(int numOfThreads = 30)
        {
            _web = new WebNewBankruptContext();
            _parsed = new ParsedNewBakruptContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }

        public override async Task StartMigratingAsync()
        {
            await MigrateBankruptCompleted();
            await MigrateBankruptAtStage();
            await MigrateRehabilityCompleted();
            await using var parsed = new ParsedNewBakruptContext();
            await parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.bankrupt_completed, avroradata.bankrupt_at_stage,avroradata.rehability_completed restart identity;");

        }

        private async Task MigrateBankruptCompleted()
        {
            await foreach (var bankruptCompleted in _parsed.NewBankruptCompleteds)
            {
                var newBankruptCompleted = new NewBankruptCompleted
                {
                    Bin = bankruptCompleted.Bin,
                    DateDecision = bankruptCompleted.DateDecision,
                    DateEntry = bankruptCompleted.DateEntry,
                    DateDecisionEnd = bankruptCompleted.DateDecisionEnd,
                    DateEntryEnd = bankruptCompleted.DateEntryEnd,
                    DateOfRelevance = bankruptCompleted.DateOfRelevance
                };
                await _web.NewBankruptCompleteds.Upsert(newBankruptCompleted).On(x => x.Bin).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
        private async Task MigrateBankruptAtStage()
        {
            await foreach (var newBankruptAtStage in _parsed.NewBankruptAtStages)
            {
                var bankruptAtStage = new NewBankruptAtStage
                {
                    Bin = newBankruptAtStage.Bin,
                    DateOfCourtDecision = newBankruptAtStage.DateOfCourtDecision,
                    DateOfEntryIntoForce = newBankruptAtStage.DateOfEntryIntoForce,
                    DateOfRelevance = newBankruptAtStage.DateOfRelevance
                };
                await _web.NewBankruptAtStages.Upsert(bankruptAtStage).On(x => x.Bin).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
        private async Task MigrateRehabilityCompleted()
        {
            foreach (var newRehabilityCompleted in _parsed.NewRehabilityCompleteds)
            {
                var rehabilityCompleted = new NewRehabilityCompleted
                {
                    Bin = newRehabilityCompleted.Bin,
                    DateDecision = newRehabilityCompleted.DateDecision,
                    DateEntry = newRehabilityCompleted.DateEntry,
                    DateDecisionEnd = newRehabilityCompleted.DateDecisionEnd,
                    DateEntryEnd = newRehabilityCompleted.DateEntryEnd,
                    DateOfRelevance = newRehabilityCompleted.DateOfRelevance
                };
                await _web.NewRehabilityCompleteds.Upsert(rehabilityCompleted).On(x => x.Bin).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
    
}