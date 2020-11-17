using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class KgdSearchedTaxpayersMigrationService : MigrationService
    {
        private readonly WebKgdSearchedTaxpayersContext _web;
        private readonly ParsedKgdSearchedTaxpayersContext _parsed;
        private readonly object _forLock;
        private int _counter;

        public KgdSearchedTaxpayersMigrationService(int numOfThreads = 30)
        {
            _web = new WebKgdSearchedTaxpayersContext();
            _parsed = new ParsedKgdSearchedTaxpayersContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        public override async Task StartMigratingAsync()
        {
            await Migrate();
            await using var parsed = new ParsedKgdSearchedTaxpayersContext();
            await parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.kgd_searched_taxpayers restart identity");
        }

        private async Task Migrate()
        {
            foreach (var taxpayersDto in _parsed.KgdSearchedTaxpayersDto)
            {
                var kgdSearchedTaxpayers = new KgdSearchedTaxpayers
                {
                    IinBin = taxpayersDto.IinBin,
                    Number = taxpayersDto.Number,
                    OrgName = taxpayersDto.OrgName,
                    PostedDate = taxpayersDto.PostedDate,
                    PreparationDate = taxpayersDto.PreparationDate
                };
                await _web.KgdSearchedTaxpayerses.Upsert(kgdSearchedTaxpayers).On(x => x.IinBin).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
    
}