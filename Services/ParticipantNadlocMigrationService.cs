
using System;
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
    public class ParticipantNadlocMigrationService : MigrationService
    {
        private int _total;
        private int _total2;
        private int _total3;
        private DateTime _start_date = DateTime.Now; 
        private readonly object _lock = new object();

        public ParticipantNadlocMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedParticipantNadlocContext = new ParsedParticipantNadlocContext();
            _total = parsedParticipantNadlocContext.ParticipantNadlocDtos.Count();
            _total2 = parsedParticipantNadlocContext.CustomersNadlocDtos.Count();
            _total3 = parsedParticipantNadlocContext.SupplierNadlocDtos.Count();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webParticipantNadlocContext = new WebParticipantNadlocContext();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            webParticipantNadlocContext.SupplierNadloc.RemoveRange(webParticipantNadlocContext.SupplierNadloc
                .Where(x => x.RelevanceDate < _start_date));
            webParticipantNadlocContext.SaveChanges();
            Logger.Info("End of migration");
        }

        private  async Task Migrate(int threadNum)
        {
            Logger.Info("startted thread");
            
            await using var webParticipantNadlocContext = new WebParticipantNadlocContext();
            await using var parsedParticipantNadlocContext = new ParsedParticipantNadlocContext();

            /*foreach (var dto in parsedParticipantNadlocContext.ParticipantNadlocDtos.Where(x=>x.Id % NumOfThreads==threadNum))
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
            }*/
            foreach (var dto in parsedParticipantNadlocContext.SupplierNadlocDtos.Where(x=>x.Id% NumOfThreads==threadNum).Where(x=>x.Status==true).Select(x=>new SupplierNadloc()
            {
                Id = x.Id,
                Bin = x.Bin,
                RelevanceDate = DateTime.Now,
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