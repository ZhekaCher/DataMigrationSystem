﻿using System;
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
    public class TerroristMigrationService:MigrationService
    {
        private readonly WebTerroristContext _webTerroristContext;
        private readonly ParsedTerroristContext _parsedTerroristContext;

        public TerroristMigrationService(int numOfThreads = 1)
        {
            NumOfThreads = numOfThreads;
            _webTerroristContext = new WebTerroristContext();
            _parsedTerroristContext = new ParsedTerroristContext();
        }

        public override async Task StartMigratingAsync()
        {
            /*var terroristDtos = _parsedTerroristContext.TerroristDtos
                .Select(x => new Terrorist
                {
                    Id = x.Id,
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    MiddleName = x.MiddleName,
                    Birthday = x.Birthday,
                    Note = x.Note,
                    Correction = x.Correction,
                    Iin = x.Iin,
                    RelevanceDate = x.RelevanceDate,
                    Type = x.Status
                });*/
            var count = 0;
            var terroristDtos = _parsedTerroristContext.TerroristDtos.Join(_parsedTerroristContext.IndividualIins,
                terroristDto => terroristDto.Iin, individual => individual.Code,
                (terroristDto, individual) => new Terrorist
                {
                    Id = terroristDto.Id,
                    LastName = terroristDto.LastName,
                    FirstName = terroristDto.FirstName,
                    MiddleName = terroristDto.MiddleName,
                    Birthday = terroristDto.Birthday,
                    Note = terroristDto.Note,
                    Correction = terroristDto.Correction,
                    Iin = terroristDto.Iin,
                    RelevanceDate = terroristDto.RelevanceDate,
                    Type = terroristDto.Status
                });
            foreach (var terroristDto in terroristDtos)
                {
                    await using var terroristInsertContext =  new WebTerroristContext();
                    await terroristInsertContext.Terrorists.Upsert(terroristDto).On(x => new{x.Iin, x.Type}).RunAsync();
                    Logger.Trace(++count);
                }
                var minDate = await _parsedTerroristContext.TerroristDtos.MinAsync(x => x.RelevanceDate);
                _webTerroristContext.Terrorists.RemoveRange(_webTerroristContext.Terrorists.Where(x=>x.RelevanceDate<minDate));
                await _webTerroristContext.SaveChangesAsync();
                await _parsedTerroristContext.Database.ExecuteSqlRawAsync("truncate avroradata.terrorists restart identity;");
        }
    }
}