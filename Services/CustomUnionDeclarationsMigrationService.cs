﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class CustomUnionDeclarationsMigrationService : MigrationService
    {
        private readonly WebCustomUnionDeclarationsContext _webCustomUnionDeclarationsContex;
        private readonly ParsedCustomUnionDeclarations _parsedCustomUnionDeclarations;
        private readonly object _forLock;
        private int _counter;

        public CustomUnionDeclarationsMigrationService(int numOfThreads = 1) 
        {
            _webCustomUnionDeclarationsContex = new WebCustomUnionDeclarationsContext();
            _parsedCustomUnionDeclarations = new ParsedCustomUnionDeclarations();
            NumOfThreads = numOfThreads;
            _forLock = new object();
        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }
        public override async Task StartMigratingAsync()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(MigarteAd(i));
                tasks.Add(Migrate(i));
                if (tasks.Count < NumOfThreads*2) continue;
                await Task.WhenAny(tasks);
                tasks.RemoveAll(x => x.IsCompleted);
            }
            await Task.WhenAll(tasks);

            await using var parsedCustomUnionDeclarations = new ParsedCustomUnionDeclarations();
            await parsedCustomUnionDeclarations.Database.ExecuteSqlRawAsync("truncate avroradata.custom_union_declarations_ad,avroradata.custom_union_declarations restart identity;");
        }

        private async Task Migrate(int threadNum)
        {
            var parseCud = _parsedCustomUnionDeclarations.CustomUnionDeclarationsDtos;
            foreach (var customUnionDeclarationsDto in parseCud.Where(x=>x.Id % NumOfThreads == threadNum))
            {
                var cUnDic = new CustomUnionDeclarations
                {
                    CertOrgAddress = customUnionDeclarationsDto.CertOrgAddress,
                    CertOrgHeader = customUnionDeclarationsDto.CertOrgHeader,
                    CertOrgName = customUnionDeclarationsDto.CertOrgName,
                    DeclarantsAddress = customUnionDeclarationsDto.DeclarantsAddress,
                    DeclarantsIinBiin = customUnionDeclarationsDto.DeclarantsIinBiin,
                    DeclarantsName = customUnionDeclarationsDto.DeclarantsName,
                    Details = customUnionDeclarationsDto.Details,
                    EvidanceDoc = customUnionDeclarationsDto.EvidanceDoc,
                    ExtensionInform = customUnionDeclarationsDto.ExtensionInform,
                    RegNum = customUnionDeclarationsDto.RegNum,
                    ValFrom = customUnionDeclarationsDto.ValFrom,
                    ValUntil = customUnionDeclarationsDto.ValUntil ,
                    ManufacturersName = customUnionDeclarationsDto.ManufacturersName,
                    ManufacturersAdress = customUnionDeclarationsDto.ManufacturersAdress,
                    ProductName = customUnionDeclarationsDto.ProductName,
                    IdentificateInform = customUnionDeclarationsDto.IdentificateInform,
                    TnCode = customUnionDeclarationsDto.TnCode,
                    Npa = customUnionDeclarationsDto.Npa,
                    RelevanceDate = customUnionDeclarationsDto.RelevanceDate
                };
                await _webCustomUnionDeclarationsContex.CustomUnionDeclarationses.Upsert(cUnDic).On(x => x.RegNum)
                    .RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
                
            }
        }

        private async Task MigarteAd(int threadNum)
        {
            var customUnionDeclarationsAdDtos = _parsedCustomUnionDeclarations.CustomUnionDeclarationsAdDtos;
            foreach (var customUnionDeclarationsAdDto in customUnionDeclarationsAdDtos.Where(x=>x.Id % NumOfThreads == threadNum))
            {
                var t = new CustomUnionDeclarationsAd
                {
                    Name = customUnionDeclarationsAdDto.Name,
                    CtRk = customUnionDeclarationsAdDto.CtRk,
                    TnVad = customUnionDeclarationsAdDto.TnVad,
                    Declarations = customUnionDeclarationsAdDto.Declarations,
                    RelevanceDate = customUnionDeclarationsAdDto.RelevanceDate
                };
                await _webCustomUnionDeclarationsContex.CustomUnionDeclarationsAds.Upsert(t).On(x => new {x.Name,x.Declarations})
                    .RunAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}