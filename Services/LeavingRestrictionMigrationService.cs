using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public sealed class LeavingRestrictionMigrationService : MigrationService
    {

        private long _counter;
        private readonly object _forLock = new object();
        public LeavingRestrictionMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var webLeavingRestrictionContext = new WebLeavingRestrictionContext();
            await using var parsedLeavingRestrictionContext = new  ParsedLeavingRestrictionContext();
            var companyDtos = from leavingRestrictionDto in parsedLeavingRestrictionContext.LeavingRestrictionDtos
                where leavingRestrictionDto.IinBin%NumOfThreads==numThread
                orderby leavingRestrictionDto.IinBin
                select new LeavingRestriction
                {
                    Date = leavingRestrictionDto.Date,
                    JudicialExecutor = leavingRestrictionDto.JudicialExecutor,
                    IinBin = leavingRestrictionDto.IinBin,
                    Debtor = leavingRestrictionDto.Debtor,
                    RelevanceDate = leavingRestrictionDto.RelevanceDate,
                    Cause = leavingRestrictionDto.Cause
                };

            long bin = 0;
            var oldCounter = 0;
            var newCounter = 0;
            foreach (var companyDto in companyDtos)
            {
                if (bin != companyDto.IinBin)
                { 
                    if (oldCounter!=newCounter)
                    {
                        await webLeavingRestrictionContext.Database.ExecuteSqlInterpolatedAsync($"insert into avroradata.leaving_restriction_history (biin, count) values ({bin}, {oldCounter});");
                    }                    
                    bin = companyDto.IinBin;
                    oldCounter = await webLeavingRestrictionContext.LeavingRestrictions.CountAsync(x => x.IinBin == bin);
                    webLeavingRestrictionContext.LeavingRestrictions.RemoveRange(
                        webLeavingRestrictionContext.LeavingRestrictions.Where(x =>
                            x.IinBin == bin));
                    await webLeavingRestrictionContext.SaveChangesAsync();
                    newCounter = 0;
                }
                await webLeavingRestrictionContext.LeavingRestrictions.AddAsync(companyDto);
                await webLeavingRestrictionContext.SaveChangesAsync();
                newCounter++;
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
                }
            }
        }
        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            
            for (var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
                
            }

            await Task.WhenAll(tasks);
            await using var parsedLeavingRestrictionContext = new  ParsedLeavingRestrictionContext();
            await using var webLeavingRestrictionContext = new  WebLeavingRestrictionContext();
            await parsedLeavingRestrictionContext.Database.ExecuteSqlRawAsync("truncate avroradata.leaving_restriction restart identity;");
            await webLeavingRestrictionContext.Database.ExecuteSqlRawAsync($"call avroradata.unreliable_companies_updater();");

        }
    }
}