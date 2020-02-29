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
    /// @date 29.02.2020 15:20:03
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class DirectorGoszakupMigrationService : MigrationService
    {
        private int _total;
        private object _lock = new object();
        //TODO(Get source by code 'goszakup')
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public DirectorGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedDirectorGoszakupContext = new ParsedDirectorGoszakupContext();
            _total = parsedDirectorGoszakupContext.DirectorGoszakupDtos.Count();
        }
        
        public async override Task StartMigratingAsync()
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


            await using var webCompanyDirectorContext = new WebCompanyDirectorContext();
            await using var parsedDirectorGoszakupContext = new ParsedDirectorGoszakupContext();
            foreach (var dto in parsedDirectorGoszakupContext.DirectorGoszakupDtos)
            {
                var temp = DtoToWeb(dto);
                try
                {
                    Logger.Trace($"Moving: {dto.Bin}");
                    await webCompanyDirectorContext.CompanyDirectors.Upsert(temp)
                        .On(x => new{x.CompanyBin, x.DirectorIin}).RunAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        Logger.Warn($"Message:|{e.Message}|; Bin:|{temp.CompanyBin}|; Iin:|{temp.DirectorIin}|");
                    }
                    else
                    {
                        Logger.Error(
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; Biin:|{temp.CompanyBin}|;");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            Logger.Info($"Completed thread at {_total}");
        }
        
        
        
        private CompanyDirector DtoToWeb(DirectorGoszakupDto directorGoszakupDto)
        {
            var companyDirector = new CompanyDirector();
            companyDirector.CompanyBin = directorGoszakupDto.Bin;
            companyDirector.DirectorIin = directorGoszakupDto.Iin;
            companyDirector.IDatasource = 2;
            return companyDirector;
        }
    }
}