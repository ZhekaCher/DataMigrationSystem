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
    /// INPUT
    /// </summary>
    public class AllParticipantsGoszakupMigrationService : MigrationService
    {
        private int _total;
        private object _lock = new object();

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public AllParticipantsGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedAllParticipantsGoszakupContext = new ParsedAllParticipantsGoszakupContext();
            _total = parsedAllParticipantsGoszakupContext.AllParticipantsGoszakupDtos.Count();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");


            await using var webAllParticipantsGoszakupContext = new WebAllParticipantsGoszakupContext();
            await using var parsedAllParticipantsGoszakupContext = new ParsedAllParticipantsGoszakupContext();
            foreach (var dto in parsedAllParticipantsGoszakupContext.AllParticipantsGoszakupDtos.Where(x =>
                    x.Pid % NumOfThreads == threadNum)
                .Where(x => x.Inn == null && x.Unp == null && (x.Bin != null || x.Iin != null)))
            {
                var temp = DtoToWeb(dto);
                try
                {
                    await webAllParticipantsGoszakupContext.AllParticipantsGoszakup.Upsert(temp)
                        .On(x => x.BiinCompanies).RunAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        Logger.Warn($"Message:|{e.Message}|; Biin:|{temp.BiinCompanies}|;");
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


            Logger.Info("Completed thread");
        }

        private AllParticipantsGoszakup DtoToWeb(AllParticipantsGoszakupDto allParticipantsGoszakupDto)
        {
            var allParticipantsGoszakup = new AllParticipantsGoszakup();
            var biin = allParticipantsGoszakupDto.Bin ?? allParticipantsGoszakupDto.Iin;
            allParticipantsGoszakup.Crdate = allParticipantsGoszakupDto.Crdate;
            allParticipantsGoszakup.Customer = allParticipantsGoszakupDto.Customer;
            allParticipantsGoszakup.Organizer = allParticipantsGoszakupDto.Organizer;
            allParticipantsGoszakup.Pid = allParticipantsGoszakupDto.Pid;
            allParticipantsGoszakup.Regdate = allParticipantsGoszakupDto.Regdate;
            allParticipantsGoszakup.Status = true;
            allParticipantsGoszakup.Supplier = allParticipantsGoszakupDto.Supplier;
            allParticipantsGoszakup.Year = allParticipantsGoszakupDto.Year;
            allParticipantsGoszakup.BiinCompanies = biin;
            allParticipantsGoszakup.IndexDate = allParticipantsGoszakupDto.IndexDate;
            allParticipantsGoszakup.NumberReg = allParticipantsGoszakupDto.NumberReg;
            allParticipantsGoszakup.RelevanceDate = allParticipantsGoszakupDto.Relevance;
            allParticipantsGoszakup.IsSingleOrg = allParticipantsGoszakupDto.IsSingleOrg;
            allParticipantsGoszakup.LastUpdateDate = allParticipantsGoszakupDto.LastUpdateDate;
            allParticipantsGoszakup.MarkNationalCompany = allParticipantsGoszakupDto.MarkNationalCompany;
            return allParticipantsGoszakup;
        }
    }
}