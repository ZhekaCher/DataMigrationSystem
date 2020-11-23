
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class ParticipantNadlocMigrationService : MigrationService
    {
        private int _total;
        private int _total2;
        private int _total3;
        private readonly object _lock = new object();

        public ParticipantNadlocMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedParticipantNadlocContext = new ParsedParticipantNadlocContext();
            _total = parsedParticipantNadlocContext.ParticipantNadlocDtos.Count();
            _total2 = parsedParticipantNadlocContext.CustomersNadlocDtos.Count();
            _total3 = parsedParticipantNadlocContext.SupplierNadlocDtos.Count();

        }

        public override async Task StartMigratingAsync()
        {
            await using var webParticipantNadlocContext = new WebParticipantNadlocContext();
            await using var parsedParticipantNadlocContext = new ParsedParticipantNadlocContext();
            DateTime? startDate1 = await parsedParticipantNadlocContext.CustomersNadlocDtos.MinAsync(x=>x.RelevanceDate);
            DateTime? startDate2 = await parsedParticipantNadlocContext.SupplierNadlocDtos.MinAsync(x=>x.RelevanceDate);
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            webParticipantNadlocContext.SupplierNadloc.RemoveRange(webParticipantNadlocContext.SupplierNadloc
                .Where(x => x.RelevanceDate < startDate2));
            await webParticipantNadlocContext.SaveChangesAsync();
            webParticipantNadlocContext.CustomersNadloc.RemoveRange(webParticipantNadlocContext.CustomersNadloc
                .Where(x => x.RelevanceDate < startDate1));
            await webParticipantNadlocContext.SaveChangesAsync();
            
            await parsedParticipantNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_customers restart identity cascade;");
            await parsedParticipantNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_suppliers restart identity cascade;");
            await parsedParticipantNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_participants restart identity cascade;");
            await parsedParticipantNadlocContext.Database.ExecuteSqlRawAsync("truncate table avroradata.nadloc_tenders restart identity cascade;");
            Logger.Info("Truncated");
            Logger.Info("End of migration");
        }

        private  async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            
            await using var webParticipantNadlocContext = new WebParticipantNadlocContext();
            await using var parsedParticipantNadlocContext = new ParsedParticipantNadlocContext();
            await using var webContactContext = new WebContactContext();
            foreach (var dto in parsedParticipantNadlocContext.ParticipantNadlocDtos.Where(x=>x.Id % NumOfThreads==threadNum))
            {
                var temp = DtoToWeb(dto);
                await webParticipantNadlocContext.ParticipantsNadloc.Upsert(temp).On(x => x.Bin).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }

            foreach (var dto in parsedParticipantNadlocContext.CustomersNadlocDtos.Where(x=>x.Id% NumOfThreads==threadNum).Select(x=>new CustomerNadloc
            {
                Id = x.Id,
                Bin = x.Bin,
                RelevanceDate = x.RelevanceDate,
                Status = x.Status
            }))
            {
                await webParticipantNadlocContext.CustomersNadloc.Upsert(dto).On(x => x.Bin).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total2}");
            }
            foreach (var dto in parsedParticipantNadlocContext.SupplierNadlocDtos.Where(x=>x.Id% NumOfThreads==threadNum).Where(x=>x.RelevanceDate!=null).Select(x=>new SupplierNadloc()
            {
                Id = x.Id,
                Bin = x.Bin,
                RelevanceDate = x.RelevanceDate,
                Status = x.Status
            }))
            {
                await webParticipantNadlocContext.SupplierNadloc.Upsert(dto).On(x => x.Bin).RunAsync();
                lock (_lock)
                    Logger.Trace($"Left {--_total3}");
            }

        }

        private ParticipantNadloc DtoToWeb(ParticipantNadlocDto participantNadlocDto)
        {
            var participantNadloc = new ParticipantNadloc();
            participantNadloc.Id = participantNadlocDto.Id;
            participantNadloc.Bin = participantNadlocDto.Bin;
            participantNadloc.Email = participantNadlocDto.Email;
            participantNadloc.WebSite = participantNadlocDto.WebSite;
            participantNadloc.Fax = participantNadlocDto.Fax;
            participantNadloc.Incorporation = participantNadlocDto.Incorporation;
            participantNadloc.NameKz = participantNadlocDto.NameKz;
            participantNadloc.NameRu = participantNadlocDto.NameRu;
            participantNadloc.Tel1 = participantNadlocDto.Tel1;
            participantNadloc.Tel2 = participantNadlocDto.Tel2;
            participantNadloc.LegalLocality = participantNadlocDto.LegalLocality;
            participantNadloc.LegalStreet = participantNadlocDto.LegalStreet;
            participantNadloc.LegalBuildingNum = participantNadlocDto.LegalBuildingNum;
            participantNadloc.LegalOfficeNum = participantNadlocDto.LegalOfficeNum;
            participantNadloc.ActualLocality = participantNadlocDto.ActualLocality;
            participantNadloc.ActualBuilding = participantNadlocDto.ActualBuilding;
            participantNadloc.ActualStreet = participantNadlocDto.ActualStreet;
            participantNadloc.ActualOfficeNum = participantNadlocDto.ActualOfficeNum;
            participantNadloc.ChiefName = participantNadlocDto.ChiefName;
            participantNadloc.ChiefPosition = participantNadlocDto.ChiefPosition;
            participantNadloc.ContactName = participantNadlocDto.ContactName;
            participantNadloc.ContactPosition = participantNadlocDto.ChiefPosition;
            participantNadloc.ContactTel = participantNadlocDto.ContactTel;
            participantNadloc.ContactFax = participantNadlocDto.ContactFax;
            participantNadloc.ContactEmail = participantNadlocDto.ContactEmail;
            participantNadloc.RegDate = participantNadlocDto.RegDate;
            participantNadloc.CustomerLink = participantNadlocDto.CustomerLink;
            return participantNadloc;
        }
    }
}