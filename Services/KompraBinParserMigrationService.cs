using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class KompraBinParserMigrationService : MigrationService
    {
        private readonly WebKompraBinParserContext _web;
        private readonly ParsedKompraBinParserContext _parsed;
        private readonly object _forlock;
        private int _counter;

        public KompraBinParserMigrationService(int numOfThreads = 1)
        {
            _web = new WebKompraBinParserContext();
            _parsed = new ParsedKompraBinParserContext();
            NumOfThreads = numOfThreads;
            _forlock = new object();
        }
        
        public override async Task StartMigratingAsync()
        {
            await Migrate();
            await _parsed.Database.ExecuteSqlRawAsync(
               "truncate avroradata.kompra_bin_parser restart identity cascade;");
        }
        
        
        private async Task Migrate()
        {
            foreach (var kompraBinParserDto in _parsed.KompraBinParserDto)
            {
                var kompraBinParser = new KompraBinParser
                {
                    IinBin = kompraBinParserDto.IinBin,
                    Rnn = kompraBinParserDto.Rnn,
                    Address = kompraBinParserDto.Address,
                    Owner = kompraBinParserDto.Owner,
                    OwnershipType = kompraBinParserDto.OwnershipType,
                    Kato = kompraBinParserDto.Kato,
                    Workers = kompraBinParserDto.Workers,
                    Oked = kompraBinParserDto.Oked,
                    SecondaryOked = kompraBinParserDto.SecondaryOked,
                    Okpo = kompraBinParserDto.Okpo,
                    Kbe = kompraBinParserDto.Kbe,
                    Status = kompraBinParserDto.Status,
                    RelevanceDate = kompraBinParserDto.RelevanceDate
                };
                await _web.KompraBinParser.Upsert(kompraBinParser).On(x => x.IinBin).RunAsync();
                await _web.SaveChangesAsync();
                lock (_forlock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}