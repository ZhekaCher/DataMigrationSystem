using System;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TaxDetailsMigrationService : MigrationService
    {
        private readonly WebTaxDetailsContext _webTaxDetailsContext;
        private readonly ParsedTaxDetailsContext _parsedTaxDetailsContext;
        private int _total;
        private readonly object _lock = new object();

        public TaxDetailsMigrationService(int numOfThreads=1)
        {
            NumOfThreads = numOfThreads;
            _webTaxDetailsContext = new WebTaxDetailsContext();
            _parsedTaxDetailsContext = new ParsedTaxDetailsContext();
            _total = _parsedTaxDetailsContext.TaxDetailsDtos.Count();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var taxDetailsDtos = _parsedTaxDetailsContext.TaxDetailsDtos;
            foreach (var taxDetailsDto in taxDetailsDtos)
            {
                var taxDetails = new TaxDetails
                {
                    Bin = taxDetailsDto.Bin,
                    RelevanceDate = taxDetailsDto.RelevanceDate,
                    Year = taxDetailsDto.Year,
                    Amount = taxDetailsDto.Amount*1000
                };
                await _webTaxDetailsContext.TaxDetailses.Upsert(taxDetails).On(x => new {x.Bin, x.Year}).RunAsync();
                lock (_lock)
                {
                    Logger.Trace(_total--);
                }
            }
        }
    }
}