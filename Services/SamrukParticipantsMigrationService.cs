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
                    var contacts_copies = OnlyContactsCopies(dto);
                    await webSamrukParticipantsContext.SamrukParticipantses.Upsert(samrukParticipantsDto)
                        .On(x => x.CodeBin).RunAsync();
                    try
                    {
                        await webSamrukParticipantsContext.Contacts.AddAsync(contacts);
                        await webSamrukParticipantsContext.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        await webSamrukParticipantsContext.ContactCopies.AddAsync(contacts_copies);
                        await webSamrukParticipantsContext.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                    }
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
            var minDate = parsedSamrukParticipantsContext.SamrukParticipantsDtos.Min(x => x.RelevanceDate);
            webSamrukParticipantsContext.SamrukParticipantses.RemoveRange(webSamrukParticipantsContext.SamrukParticipantses.Where(x=>x.RelevanceDate<minDate));
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
             return samrukParticipants;

        }
        private Contact OnlyContacts(SamrukParticipantsDto samrukParticipantsDto)
        {
            var contact= new Contact();
            contact.Bin = samrukParticipantsDto.CodeBin;
            contact.Telephone = samrukParticipantsDto.Phone;
            contact.Website = samrukParticipantsDto.Site;
            contact.Email = samrukParticipantsDto.Email;
            contact.Source = "samruk";
            return contact;
        }
        private Contact_copy OnlyContactsCopies(SamrukParticipantsDto samrukParticipantsDto)
        {
            var contact= new Contact_copy();
            contact.Bin = samrukParticipantsDto.CodeBin;
            contact.Telephone = samrukParticipantsDto.Phone;
            contact.Website = samrukParticipantsDto.Site;
            contact.Email = samrukParticipantsDto.Email;
            contact.Source = "samruk";
            return contact;
        }
    }
}