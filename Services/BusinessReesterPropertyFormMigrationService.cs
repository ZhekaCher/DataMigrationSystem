using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class BusinessReesterPropertyFormMigrationService : MigrationService
    {
        private readonly object _forLock;
        private int _counter;

        public BusinessReesterPropertyFormMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        { 
            await using var web = new WebBusinessReesterContext();
            await using var parsed = new ParsedBusinessReesterContext();
            
            await MigrateReferences();
            var tasks = new List<Task>();
            
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            
            await web.SaveChangesAsync();
            await parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.business_reester_property_form restart identity;");
        }

        private async Task Migrate(int threadNum)
        {
            await using var web = new WebBusinessReesterContext();
            await using var parsed = new ParsedBusinessReesterContext();
            foreach (var businessReesterDto in parsed.BusinessReesterDtos.Where(x => x.Id % NumOfThreads == threadNum))
            {
                var businessReester = await DtoToEntity(businessReesterDto, web);
                await web.BusinessReesters.Upsert(businessReester).On(x => x.Bin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateReferences()
        {
            await using var web = new WebBusinessReesterContext();
            await using var parsed = new ParsedBusinessReesterContext();
            var ownerTypes = parsed.BusinessReesterDtos.Select(x => x.OwnershipType).Distinct();
            foreach (var distinct in ownerTypes)
            {
                await web.BusinessReesterOwnershipTypes.Upsert(new BusinessReesterOwnershipType
                {
                    Name = distinct
                }).On(x => x.Name).RunAsync();
            }
        }

        private async Task<BusinessReester> DtoToEntity(BusinessReesterDto reesterDto, WebBusinessReesterContext webBusinessReesterContext)
        {
            var businessReester = new BusinessReester
            {
                Bin = reesterDto.Bin
            };
            if (reesterDto.OwnershipType != null)
            {
                businessReester.OwnershipTypeId =
                    (await webBusinessReesterContext.BusinessReesterOwnershipTypes
                        .FirstOrDefaultAsync(x => x.Name == reesterDto.OwnershipType))?.Id;
            }
            else
            {
                businessReester.OwnershipTypeId = null;
            }

            return businessReester;
        }
    }
}