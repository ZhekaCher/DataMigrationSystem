using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class DisabilitiesOrgSkMigrationService : MigrationService
    {
        private readonly WebDisabilitiesOrgSkContext _webDisabilitiesOrgSkContext;
        private readonly ParsedDisabilitiesOrgSkContext _parsedDisabilitiesOrgSkContext;

        public DisabilitiesOrgSkMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            _webDisabilitiesOrgSkContext= new WebDisabilitiesOrgSkContext();
            _parsedDisabilitiesOrgSkContext = new ParsedDisabilitiesOrgSkContext();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var disabilitiesOrgSkDtos = _parsedDisabilitiesOrgSkContext.DisabilitiesOrgSkDtos
                .Select(x=>new DisabilitiesOrgSk
                {
                    Id=x.Id,
                    Bin = x.Bin,
                    ProducerType = x.ProducerType,
                    RelevanceDate = x.RelevanceDate
                } 
                );
            foreach (var disabilitiesOrgSkDto in disabilitiesOrgSkDtos)
            {
                await _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Upsert(disabilitiesOrgSkDto).On(x => x.Bin).RunAsync();
            }
            var lastDate = _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Max(x => x.RelevanceDate).Date;
            _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.RemoveRange(_webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Where(x=>x.RelevanceDate<lastDate));
            await _webDisabilitiesOrgSkContext.SaveChangesAsync();
            await _parsedDisabilitiesOrgSkContext.Database.ExecuteSqlRawAsync("truncate avroradata.disabilities_organizations_products_sk, avroradata.disabilities_organizations_sk restart identity;");
        }
    }
}