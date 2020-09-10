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
    /// @author Yevgeniy Cherdantsev
    /// @date 22.02.2020 14:02:20
    /// @version 1.0
    /// <summary>
    /// migration of goszakup participants
    /// </summary>
    public class ParticipantGoszakupMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public ParticipantGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedParticipantGoszakupContext = new ParsedParticipantGoszakupContext();
            _total = parsedParticipantGoszakupContext.ParticipantGoszakupDtos.Count();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");

            Logger.Info("Starting removing participants who out of goszakup");
            await using var webParticipantGoszakupContext = new WebParticipantGoszakupContext();
            await using var webParticipantGoszakupContext2 = new WebParticipantGoszakupContext();
            await using var parsedParticipantGoszakupContext = new ParsedParticipantGoszakupContext();
            var firstParsedTime = parsedParticipantGoszakupContext.ParticipantGoszakupDtos.OrderBy(x => x.Relevance).FirstOrDefault().Relevance; 
            var old = webParticipantGoszakupContext.ParticipantsGoszakup.Where(x =>
                x.RelevanceDate < firstParsedTime).Where(x => x.Status==true);
            var left = old.Count();
            foreach (var participantGoszakup in old)
                try
                {
                    participantGoszakup.Status = false;
                    participantGoszakup.RelevanceDate = firstParsedTime;
                    webParticipantGoszakupContext2.ParticipantsGoszakup.Update(participantGoszakup);
                    await webParticipantGoszakupContext2.SaveChangesAsync();
                    Logger.Trace($"Removing: {participantGoszakup.BiinCompanies} Left: {--left}");
                }
                catch (Exception e)
                {
                    Logger.Error(
                        $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|;");
                    Program.NumOfErrors++;
                }
            Logger.Info("Removing done");
            await parsedParticipantGoszakupContext.Database.ExecuteSqlRawAsync("truncate table avroradata.participant_goszakup restart identity cascade;");
            Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            await using var webParticipantGoszakupContext = new WebParticipantGoszakupContext();
            await using var parsedParticipantGoszakupContext = new ParsedParticipantGoszakupContext();
         
            foreach (var dto in parsedParticipantGoszakupContext.ParticipantGoszakupDtos.Where(x =>
                    x.Pid % NumOfThreads == threadNum)
                .Where(x => x.Inn == null && x.Unp == null && (x.Bin != null || x.Iin != null)))
            {
                var temp = DtoToWeb(dto);
                try
                {
                    await webParticipantGoszakupContext.ParticipantsGoszakup.Upsert(temp)
                        .On(x => x.BiinCompanies).RunAsync();
                    
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        // Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
                    }
                    else
                    {
                        Logger.Error(
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; Biin:|{temp.BiinCompanies}|; Pid:|{temp.Pid}|");
                        Program.NumOfErrors++;
                    }
                }
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            // Logger.Info($"Completed thread at {_total}");
        }


        private ParticipantGoszakup DtoToWeb(ParticipantGoszakupDto participantGoszakupDto)
        {
            var participantGoszakup = new ParticipantGoszakup();
            var biin = participantGoszakupDto.Bin ?? participantGoszakupDto.Iin;
            participantGoszakup.Crdate = participantGoszakupDto.Crdate;
            participantGoszakup.Customer = participantGoszakupDto.Customer;
            participantGoszakup.Organizer = participantGoszakupDto.Organizer;
            participantGoszakup.Pid = participantGoszakupDto.Pid;
            participantGoszakup.Regdate = participantGoszakupDto.Regdate;
            participantGoszakup.Status = true;
            participantGoszakup.Supplier = participantGoszakupDto.Supplier;
            participantGoszakup.Year = participantGoszakupDto.Year;
            participantGoszakup.BiinCompanies = biin;
            participantGoszakup.IndexDate = participantGoszakupDto.IndexDate;
            participantGoszakup.NumberReg = participantGoszakupDto.NumberReg;
            participantGoszakup.RelevanceDate = participantGoszakupDto.Relevance;
            participantGoszakup.IsSingleOrg = participantGoszakupDto.IsSingleOrg;
            participantGoszakup.LastUpdateDate = participantGoszakupDto.LastUpdateDate;
            participantGoszakup.MarkNationalCompany = participantGoszakupDto.MarkNationalCompany;
            return participantGoszakup;
        }
        
    }
}