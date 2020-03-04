using System.Linq;
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
                    Contacts = x.Contacts,
                    Bin = x.Bin,
                    IdProducerType = 2,
                    RelevanceDate = x.RelevanceDate
                } 
                );
            foreach (var disabilitiesOrgSkDto in disabilitiesOrgSkDtos)
            {
                await _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Upsert(disabilitiesOrgSkDto).On(x => x.Bin).RunAsync();
            }

            var disabledOrgProdSkDtos =
                from disabilitiesOrgProdSkDto in _parsedDisabilitiesOrgSkContext.DisabilitiesOrganizationsProductsSkDtos
                select new DisabilitiesOrganizationsProductsSkDto
                {
                    Bin = disabilitiesOrgProdSkDto.Bin,
                    Products = disabilitiesOrgProdSkDto.Products,
                    AddingDate = disabilitiesOrgProdSkDto.AddingDate,
                    Certificates = disabilitiesOrgProdSkDto.Certificates,
                    Positions = disabilitiesOrgProdSkDto.Positions,
                    DocDate = disabilitiesOrgProdSkDto.DocDate,
                    Validity = disabilitiesOrgProdSkDto.Validity,
                    RelevanceDate = disabilitiesOrgProdSkDto.RelevanceDate
                };
            await _webDisabilitiesOrgSkContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.samruk_disabled_company_products restart identity;");

            foreach (var prod in disabledOrgProdSkDtos)
            {
                await _webDisabilitiesOrgSkContext.AddAsync(prod);
            }

            await _webDisabilitiesOrgSkContext.SaveChangesAsync();
            
            var lastDate = _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Max(x => x.RelevanceDate);
            _webDisabilitiesOrgSkContext.DisabilitiesOrgSk.RemoveRange(_webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Where(x=>x.RelevanceDate<lastDate));
            await _webDisabilitiesOrgSkContext.SaveChangesAsync();
                
            await _parsedDisabilitiesOrgSkContext.Database.ExecuteSqlRawAsync("truncate avroradata.disabilities_organizations_products_sk, avroradata.disabilities_organizations_sk restart identity;");
        }
    }
}