using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class BusinessReesterMigrationService : MigrationService
    {
        private readonly WebBusinessReesterContext _web;
        private readonly ParsedBusinessReesterContext _parsed;
        private readonly object _forLock;
        private int _counter;

        public BusinessReesterMigrationService(int numOfThreads = 30)
        {
            _web = new WebBusinessReesterContext();
            _parsed = new ParsedBusinessReesterContext();
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
            await _parsed.Database.ExecuteSqlRawAsync(
                "truncate avroradata.business_reester_property_form restart identity;");
        }

        private async Task Migrate()
        {
            await foreach (var businessReesterDto in _parsed.BusinessReesterDtos)
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
                await _web.BusinessReesters.Upsert(businessReester).On(x => new {x.Bin, x.Status}).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}