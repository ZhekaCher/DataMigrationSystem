using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
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
            _webDisabilitiesOrgSkContext= new WebDisabilitiesOrgSkContext();
            _parsedDisabilitiesOrgSkContext = new ParsedDisabilitiesOrgSkContext();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            var disabilitiesOrgSkDtos = _parsedDisabilitiesOrgSkContext.DisabilitiesOrgSkDtos;
            foreach (var disabilitiesOrgSkDto in disabilitiesOrgSkDtos)
            {
                var disOrg = await DtoToEntity(disabilitiesOrgSkDto);
                await _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Upsert(disOrg).On(x => x.Bin).RunAsync();
            }
        }

        private async Task<DisabilitiesOrgSk> DtoToEntity(DisabilitiesOrgSkDto disabilitiesOrgSkDto)
        {
            var disabilitiesOrgSk = new DisabilitiesOrgSk
            {
                Id=disabilitiesOrgSkDto.Id,
                Bin = disabilitiesOrgSkDto.Bin,
                ProducerType = disabilitiesOrgSkDto.ProducerType,
                RelevanceDate = disabilitiesOrgSkDto.RelevanceDate
            };
            return disabilitiesOrgSk;
        }
    }
}