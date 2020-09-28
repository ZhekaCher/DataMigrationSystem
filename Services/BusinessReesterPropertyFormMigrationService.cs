using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
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
                var businessReester = new BusinessReester
                {
                    Region = businessReesterDto.Region,
                    District = businessReesterDto.District,
                    Locality = businessReesterDto.Locality,
                    Industry = businessReesterDto.Industry,
                    Activity = businessReesterDto.Activity,
                    LegalAddress = businessReesterDto.LegalAddress,
                    ActualAddress = businessReesterDto.ActualAddress,
                    OrganizationalForm = businessReesterDto.OrganizationalForm,
                    OwnershipType = businessReesterDto.OwnershipType,
                    Bin = businessReesterDto.Bin,
                    EstablishedYear = businessReesterDto.EstablishedYear,
                    Status = businessReesterDto.Status,
                    Cluster = businessReesterDto.Cluster,
                    Owner = businessReesterDto.Owner
                };
                await web.BusinessReesters.Upsert(businessReester).On(x => new {x.Bin, x.Status}).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}