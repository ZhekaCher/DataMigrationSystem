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
        public LeavingRestrictionMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int numThread)
        {
            await using var leavingRestrictionContext = new WebLeavingRestrictionContext();
            await using var parsedLeavingRestrictionContext = new  ParsedLeavingRestrictionContext();
            var companyDtos = from leavingRestrictionDto in parsedLeavingRestrictionContext.LeavingRestrictionDtos
                where leavingRestrictionDto.IinBin%NumOfThreads==numThread
                orderby leavingRestrictionDto.IinBin
                select new CompanyLeavingRestriction
                {
                    Date = leavingRestrictionDto.Date,
                    JudicialExecutor = leavingRestrictionDto.JudicialExecutor,
                    IinBin = leavingRestrictionDto.IinBin,
                    Debtor = leavingRestrictionDto.Debtor,
                    RelevanceDate = leavingRestrictionDto.RelevanceDate,
                    Cause = leavingRestrictionDto.Cause
                };

            long bin = 0;
            int oldCounter = 0;
            foreach (var companyDto in companyDtos)
            {
                if (bin != companyDto.IinBin)
                { 
                    await leavingRestrictionContext.Database.ExecuteSqlInterpolatedAsync($"select avroradata.leaving_restriction_history({bin}, {oldCounter})");
                    oldCounter =
                        leavingRestrictionContext.CompanyLeavingRestrictions.Count(x => x.IinBin == companyDto.IinBin);
                    bin = companyDto.IinBin;
                    leavingRestrictionContext.CompanyLeavingRestrictions.RemoveRange(
                        leavingRestrictionContext.CompanyLeavingRestrictions.Where(x =>
                            x.IinBin == companyDto.IinBin));
                    await leavingRestrictionContext.SaveChangesAsync();
                }
                await leavingRestrictionContext.CompanyLeavingRestrictions.AddAsync(companyDto);
                await leavingRestrictionContext.SaveChangesAsync();
                Logger.Info(_counter++);
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
        }
    }
}