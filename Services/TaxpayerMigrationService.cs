using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class TaxpayerMigrationService : MigrationService
    { 
        private int _total;
        private readonly object _lock = new object();
        private readonly Dictionary<string, int?> _typeDictionary = new Dictionary<string, int?>();
        public TaxpayerMigrationService(int numOfThreads = 10)
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
            await MigrateReferences();
            await Migrate();
        }
        private async Task Insert(Taxpayer taxpayer)
        {
            // var taxpayer = await DtoToEntity(taxpayerDto);
            await using var webTaxpayerContext = new WebTaxpayerContext();
            await webTaxpayerContext.Taxpayers.Upsert(taxpayer).On(x => new {taxpayer.Uin, taxpayer.TypeId}).RunAsync();
            lock (_lock)
            {
                Logger.Trace(_total--);
            }
        }
        private async Task Migrate()
        {
            await using var parsedTaxpayerContext = new ParsedTaxpayerContext();
            var taxpayers = parsedTaxpayerContext.TaxpayerDtos.AsNoTracking().Select(x=> new Taxpayer
            {
                Rnn = x.Rnn,
                FullName = x.FullName,
                Uin = x.Uin,
                DateReg = x.DateReg,
                DateUnreg = x.DateUnreg,
                ReasonUnreg = x.ReasonUnreg,
                AddInfo = x.AddInfo,
                Period = x.Period,
                RelevanceDate = x.RelevanceDate,
                TypeId = x.Type == null ? null : _typeDictionary[x.Type]
            });
            var tasks = new List<Task>();
            foreach (var taxpayer in taxpayers)
            {
                tasks.Add(Insert(taxpayer));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);
                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }
            
            await Task.WhenAll(tasks);
        }

        private async Task MigrateReferences()
        {
            await using var webTaxpayerContext = new WebTaxpayerContext();
            await using var parsedTaxpayerContext = new ParsedTaxpayerContext();
            var types = parsedTaxpayerContext.TaxpayerDtos
                .Select(x => x.Type).Distinct();
            foreach (var type in types)
            {
                await webTaxpayerContext.TypeOfServices.Upsert(new TaxpayerType
                {
                    Name = type
                }).On(x => x.Name).RunAsync();
            }
            await foreach (var type in webTaxpayerContext.TypeOfServices)
            {
                _typeDictionary[type.Name] = type.I;
            }
        }

        // private async Task<Taxpayer> DtoToEntity(TaxpayerDto taxpayerDto)
        // {
        //     await using var webTaxpayerContext = new WebTaxpayerContext();
        //     var taxpayer = new Taxpayer
        //     {
        //         Rnn = taxpayerDto.Rnn,
        //         FullName = taxpayerDto.FullName,
        //         Uin = taxpayerDto.Uin,
        //         DateReg = taxpayerDto.DateReg,
        //         DateUnreg = taxpayerDto.DateUnreg,
        //         ReasonUnreg = taxpayerDto.ReasonUnreg,
        //         AddInfo = taxpayerDto.AddInfo,
        //         Period = taxpayerDto.Period,
        //         RelevanceDate = taxpayerDto.RelevanceDate,
        //     };
        //     if (taxpayerDto.Type != null)
        //     {
        //         var l = (await webTaxpayerContext.TypeOfServices.FirstOrDefaultAsync(x =>
        //             x.Name == taxpayerDto.Type))?.I;
        //         if (l != null)
        //             taxpayer.TypeId = l;
        //     }
        //     else
        //     {
        //         taxpayer.TypeId = null;
        //     }
        //     return taxpayer;
        // }
    }
}