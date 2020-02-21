using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public sealed class LeavingRestrictionMigrationService : MigrationService
    {
        private readonly WebLeavingRestrictionContext _leavingRestrictionContext;
        private readonly ParsedLeavingRestrictionContext _parsedLeavingRestrictionContext;

        public LeavingRestrictionMigrationService()
        {
            _leavingRestrictionContext = new WebLeavingRestrictionContext();
            _parsedLeavingRestrictionContext = new ParsedLeavingRestrictionContext();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var companyDtos = from leavingRestrictionDto in _parsedLeavingRestrictionContext.LeavingRestrictionDtos
                join companies in _parsedLeavingRestrictionContext.ParsedCompanies
                    on leavingRestrictionDto.IinBin equals companies.Bin
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

            long i = 0;
            long bin = 0;
            int oldCounter = 0;
            foreach (var companyDto in companyDtos)
            {
                if (bin != companyDto.IinBin)
                { 
                    await _leavingRestrictionContext.Database.ExecuteSqlRawAsync($"select avroradata.leaving_restriction_history({bin}, {oldCounter})");
                    oldCounter =
                        _leavingRestrictionContext.CompanyLeavingRestrictions.Count(x => x.IinBin == companyDto.IinBin);
                    bin = companyDto.IinBin;
                    _leavingRestrictionContext.CompanyLeavingRestrictions.RemoveRange(
                        _leavingRestrictionContext.CompanyLeavingRestrictions.Where(x =>
                            x.IinBin == companyDto.IinBin));
                    await _leavingRestrictionContext.SaveChangesAsync();
                }
                await _leavingRestrictionContext.CompanyLeavingRestrictions.AddAsync(companyDto);
                await _leavingRestrictionContext.SaveChangesAsync();
                Logger.Info(i++);
            }
        }
    }
}