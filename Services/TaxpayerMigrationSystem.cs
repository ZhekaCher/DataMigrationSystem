using System.Collections.Generic;
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
    public class TaxpayerMigrationSystem : MigrationService
    { 
        private int _total;
        private readonly object _lock = new object();

        public TaxpayerMigrationSystem(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedTaxpayerContext = new ParsedTaxpayerContext();
            _total = parsedTaxpayerContext.TaxpayerDtos.Count();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            await MigrateReferences();
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));

            }
            await Task.WhenAll(tasks);
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            await using var webTaxpayerContext = new WebTaxpayerContext();
            await using var parsedTaxpayerContext = new ParsedTaxpayerContext();
            var taxpayersDtos = from taxpayersDto in parsedTaxpayerContext.TaxpayerDtos
                where taxpayersDto.Uin % NumOfThreads == threadNum
                select taxpayersDto;
            foreach (var taxpayersDto in taxpayersDtos)
            {
                var taxpayer = await DtoToEntity(taxpayersDto);
                await webTaxpayerContext.Taxpayers.Upsert(taxpayer).On(x => new {taxpayer.Uin, taxpayer.TypeId}).RunAsync();
                lock (_lock)
                {
                    Logger.Trace(_total--);
                }
            }
        }

        private async Task MigrateReferences()
        {
            await using var webTaxpayerContext = new WebTaxpayerContext();
            await using var parsedTaxpayerContext = new ParsedTaxpayerContext();
            var typeOfServices = parsedTaxpayerContext.TaxpayerDtos
                .Select(x => x.Type).Distinct();
            foreach (var typeOfService in typeOfServices)
            {
                await webTaxpayerContext.TypeOfServices.Upsert(new TypeOfService
                {
                    Name = typeOfService
                }).On(x => x.Name).RunAsync();
            }
        }

        private async Task<Taxpayer> DtoToEntity(TaxpayerDto taxpayerDto)
        {
            await using var webTaxpayerContext = new WebTaxpayerContext();
            var taxpayer = new Taxpayer
            {
                Rnn = taxpayerDto.Rnn,
                FullName = taxpayerDto.FullName,
                Uin = taxpayerDto.Uin,
                DateReg = taxpayerDto.DateReg,
                DateUnreg = taxpayerDto.DateUnreg,
                ReasonUnreg = taxpayerDto.ReasonUnreg,
                AddInfo = taxpayerDto.AddInfo,
                Period = taxpayerDto.Period,
                RelevanceDate = taxpayerDto.RelevanceDate,
            };
            if (taxpayerDto.Type != null)
            {
                var l = (await webTaxpayerContext.TypeOfServices.FirstOrDefaultAsync(x =>
                    x.Name == taxpayerDto.Type))?.I;
                if (l != null)
                    taxpayer.TypeId = (int) l;
            }
            else
            {
                taxpayer.TypeId = null;
            }
            return taxpayer;
        }
    }
}