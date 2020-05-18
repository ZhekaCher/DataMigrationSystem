using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using NLog;
using System.Linq;
using DataMigrationSystem.Models.Parsed;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Services
{
    public class SamrukParticipantsMigrationService:MigrationService
    {


        private readonly  object _forLock  = new object();
        private int _counter;
        public SamrukParticipantsMigrationService(int numOfThreads = 20)
        {
            NumOfThreads = numOfThreads;
        }
        
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        private async Task MigrateAsync(int threadNum)
        {
            {
                var webSamrukParticipantsContext = new WebSamrukParticipantsContext();
                var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
                foreach (var dto in parsedSamrukParticipantsContext.SamrukParticipantsDtos.Where(x=>x.Id % NumOfThreads == threadNum))
                {
                    var samrukParticipantsDto = DtoToWeb(dto);
                    var contacts = OnlyContacts(dto);
                    await webSamrukParticipantsContext.SamrukParticipantses.Upsert(samrukParticipantsDto)
                        .On(x => x.CodeBin).RunAsync();
                    await webSamrukParticipantsContext.Contacts.Upsert(contacts).On(x=>new {x.Bin, x.Source}).NoUpdate().RunAsync();
                    lock (_forLock)
                    {
                        Logger.Trace(_counter++);
                    }
                }

            }
        }

        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigrateAsync(i));
            }
            await Task.WhenAll(tasks);
            
            var  webSamrukParticipantsContext = new WebSamrukParticipantsContext();
            var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
            var minDate = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.MinAsync(x => x.RelevanceDate);
            webSamrukParticipantsContext.SamrukParticipantses.RemoveRange(webSamrukParticipantsContext.SamrukParticipantses.Where(x=>x.RelevanceDate<minDate));
            
            
            var webProducerSkContext = new WebProducerSkContext();
            var parsedProducerSkContext = new ParsedProducerSkContext();
            var lastDateP = await parsedProducerSkContext.ProducerSkDtos.MinAsync(x => x.RelevanceDate);
            webProducerSkContext.ProducerSks.RemoveRange(webProducerSkContext.ProducerSks.Where(x=>x.RelevanceDate<lastDateP));
            
            var parsedDisabilitiesOrgSkContext = new ParsedDisabilitiesOrgSkContext();
            var webDisabilitiesOrgSkContext= new WebDisabilitiesOrgSkContext();
            var lastDateD = await parsedDisabilitiesOrgSkContext.DisabilitiesOrgSkDtos.MinAsync(x => x.RelevanceDate);
            webDisabilitiesOrgSkContext.DisabilitiesOrgSk.RemoveRange(webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Where(x=>x.RelevanceDate<lastDateD));
            
            var parsedReliableSkContext = new ParsedReliableSkContext();
            var webReliableSkContext = new WebReliableSkContext();
            var lastDateR = await parsedReliableSkContext.ReliableSkDtos.MinAsync(x => x.RelevanceDate);
            webReliableSkContext.ReliableSks.RemoveRange(webReliableSkContext.ReliableSks.Where(x=>x.RelevanceDate<lastDateR));
            
            var parsedUnreliableSkContext = new ParsedUnreliableSkContext();
            var webUnreliableSkContext = new WebUnreliableSkContext();
            var lastDateU = await parsedUnreliableSkContext.UnreliableSkDtos.MinAsync(x => x.RelevanceDate);
            webUnreliableSkContext.UnreliableSks.RemoveRange(webUnreliableSkContext.UnreliableSks.Where(x=>x.RelevanceDate<lastDateU));
            
            await webSamrukParticipantsContext.SaveChangesAsync();
            await parsedSamrukParticipantsContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.samruk_all_participants restart identity");
        }

        private SamrukParticipants DtoToWeb(SamrukParticipantsDto samrukParticipantsDto)
        {
            var samrukParticipants= new SamrukParticipants();
            samrukParticipants.CodeBin = samrukParticipantsDto.CodeBin;
             samrukParticipants.DirectorFullname = samrukParticipantsDto.DirectorFullname;
             samrukParticipants.DirectorIin = samrukParticipantsDto.DirectorIin;
             samrukParticipants.Customer = samrukParticipantsDto.Customer;
             samrukParticipants.Supplier = samrukParticipantsDto.Supplier;
             samrukParticipants.RelevanceDate = samrukParticipantsDto.RelevanceDate;
             samrukParticipants.DisabledCompany = samrukParticipantsDto.InvalidCompany;
             samrukParticipants.ProducerCompany = samrukParticipants.ProducerCompany;
             samrukParticipants.UnreliableCompany = samrukParticipantsDto.GenuineSupplier;
             samrukParticipants.ReliableCompany = samrukParticipantsDto.BadSupplier;
             return samrukParticipants;
        }
        private Contact OnlyContacts(SamrukParticipantsDto samrukParticipantsDto)
        {
            var contact = new Contact
            {
                Bin = samrukParticipantsDto.CodeBin,
                Telephone = samrukParticipantsDto.Phone,
                Website = samrukParticipantsDto.Site,
                Email = samrukParticipantsDto.Email,
                Source = "samruk"
            };
            return contact;
        }
    }
}