using System;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class EgovAutoInfoMigrationService : MigrationService
    {
        private readonly WebEgovAutoInfoContext _webEgov;
        private readonly ParsedEgovAutoInfoContext _parsedEgov;
        private readonly object _forLock;
        private int _counter;

        public EgovAutoInfoMigrationService(int numOfThreads = 1)
        {
            _webEgov = new WebEgovAutoInfoContext();
            _parsedEgov = new ParsedEgovAutoInfoContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        
        public override async Task StartMigratingAsync()
        {
            await Migrate();
            var parsedEgovAutoInfoContext = new ParsedEgovAutoInfoContext();
            await parsedEgovAutoInfoContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.egov_avto_info restart identity;");

        }

        private async Task Migrate()
        {
            var parsedEgov = _parsedEgov.EgovAutoInfoDtos;
            await foreach (var egovAutoInfoDto in parsedEgov)
            {
                var egovAutoInfo = new EgovAutoInfoDto
                {
                    ArticleName = egovAutoInfoDto.ArticleName,
                    Content = egovAutoInfoDto.Content
                };
                await _webEgov.EgovAutoInfoDtos.Upsert(egovAutoInfo).On(x=>x.ArticleName).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}