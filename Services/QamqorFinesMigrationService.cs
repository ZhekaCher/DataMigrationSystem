using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class QamqorFinesMigrationService : MigrationService
    {
        private readonly WebQamqorFinesContext _webQamqorFines;
        private readonly ParsedQamqorFinesContext _parsedQamqorFines;
        private readonly object _forLock;
        private int _counter;

        public QamqorFinesMigrationService(int numOfThreads = 30)
        {
            _webQamqorFines = new WebQamqorFinesContext();
            _parsedQamqorFines = new ParsedQamqorFinesContext();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        
        public override async Task StartMigratingAsync()
        {
            await Migrate();
            var parsedQamqorFinesContext = new ParsedQamqorFinesContext();
            await parsedQamqorFinesContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.qamqor_fines restart identity;");

        }

        private async Task Migrate()
        {
            var qamqorFinesDtos = _parsedQamqorFines.QamqorFinesDtos;
            await foreach (var qamqorFinesDto in qamqorFinesDtos)
            {
                var qamqorFines = new QamqorFines
                {
                    NumAdministrative = qamqorFinesDto.NumAdministrative,
                    AdministrativeFine = qamqorFinesDto.AdministrativeFine,
                    Authority = qamqorFinesDto.Authority,
                    CommissionDate = qamqorFinesDto.CommissionDate,
                    ConsiderationDate = qamqorFinesDto.ConsiderationDate,
                    FineStatus = qamqorFinesDto.FineStatus,
                    GosNumber = qamqorFinesDto.GosNumber,
                    MainMeasure = qamqorFinesDto.MainMeasure
                };
                await _webQamqorFines.QamqorFineses.Upsert(qamqorFines)
                    .On(x => new {x.NumAdministrative}).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}