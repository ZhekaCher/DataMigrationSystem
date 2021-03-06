﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using NLog;
using System.Linq;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
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
        

        private async Task MigrateAsync(int threadNum)
        {
            var webSamrukParticipantsContext = new WebSamrukParticipantsContext();
            var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
            var samrukParticipants = parsedSamrukParticipantsContext.SamrukParticipantsDtos
                .Join(parsedSamrukParticipantsContext.CompanyBinDtos,
                    samrukParticipantsDto => samrukParticipantsDto.CodeBin, companyBinDto => companyBinDto.Code,
                    (samrukParticipantsDto, companyBinDto) => new {samrukParticipantsDto, companyBinDto})
                .Where(x => x.samrukParticipantsDto.Id % NumOfThreads == threadNum);
            foreach (var dto in samrukParticipants)
            {
                var samrukParticipantsDto = DtoToWeb(dto.samrukParticipantsDto);
                await webSamrukParticipantsContext.SamrukParticipantses.Upsert(samrukParticipantsDto)
                    .On(x => x.CodeBin).RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter++);
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
            
            await using var parsedSamrukParticipantsContext = new ParsedSamrukParticipantsContext();
            
            await using var webProducerSkContext = new WebProducerSkContext();
            var lastDateP = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.Where(x=>x.CommodityProducer==true).MinAsync(x => x.RelevanceDate);
            webProducerSkContext.ProducerSks.RemoveRange(webProducerSkContext.ProducerSks.Where(x=>x.RelevanceDate<lastDateP));
            
            await using var webDisabilitiesOrgSkContext= new WebDisabilitiesOrgSkContext();
            var lastDateD = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.Where(x=>x.InvalidCompany==true).MinAsync(x => x.RelevanceDate);
            webDisabilitiesOrgSkContext.DisabilitiesOrgSk.RemoveRange(webDisabilitiesOrgSkContext.DisabilitiesOrgSk.Where(x=>x.RelevanceDate<lastDateD));
            
            await using var webReliableSkContext = new WebReliableSkContext();
            var lastDateR = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.Where(x=>x.GenuineSupplier==true).MinAsync(x => x.RelevanceDate);
            webReliableSkContext.ReliableSks.RemoveRange(webReliableSkContext.ReliableSks.Where(x=>x.RelevanceDate<lastDateR));
            
            await using var webUnreliableSkContext = new WebUnreliableSkContext();
            var lastDateU = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.Where(x=>x.BadSupplier==true).MinAsync(x => x.RelevanceDate);
            webUnreliableSkContext.UnreliableSks.RemoveRange(webUnreliableSkContext.UnreliableSks.Where(x=>x.RelevanceDate<lastDateU));
            
            await using var webSamrukParticipantsContext = new WebSamrukParticipantsContext();
            var minDate = await parsedSamrukParticipantsContext.SamrukParticipantsDtos.MinAsync(x => x.RelevanceDate);
            webSamrukParticipantsContext.SamrukParticipantses.RemoveRange(webSamrukParticipantsContext.SamrukParticipantses.Where(x=>x.RelevanceDate<minDate));
            
            await webSamrukParticipantsContext.SaveChangesAsync();
            await webUnreliableSkContext.SaveChangesAsync();
            await webReliableSkContext.SaveChangesAsync();
            await webDisabilitiesOrgSkContext.SaveChangesAsync();
            await webProducerSkContext.SaveChangesAsync();
            
            await parsedSamrukParticipantsContext.Database.ExecuteSqlRawAsync("truncate avroradata.samruk_all_participants restart identity");
        }

        private SamrukParticipants DtoToWeb(SamrukParticipantsDto samrukParticipantsDto)
        {
            var samrukParticipants = new SamrukParticipants
            {
                CodeBin = samrukParticipantsDto.CodeBin,
                DirectorFullname = samrukParticipantsDto.DirectorFullname,
                DirectorIin = samrukParticipantsDto.DirectorIin,
                Customer = samrukParticipantsDto.Customer,
                Supplier = samrukParticipantsDto.Supplier,
                RelevanceDate = samrukParticipantsDto.RelevanceDate,
                DisabledCompany = samrukParticipantsDto.InvalidCompany,
                ProducerCompany = samrukParticipantsDto.CommodityProducer,
                UnreliableCompany = samrukParticipantsDto.BadSupplier,
                ReliableCompany = samrukParticipantsDto.GenuineSupplier
            };
            return samrukParticipants;
        }
    }
}