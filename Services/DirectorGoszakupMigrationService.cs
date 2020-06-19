using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            _total = parsedDirectorGoszakupContext.DirectorGoszakupDtos.Count(x => x.Bin != null && x.Iin != null);
        }

        public async override Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            await using var parsedDirectorGoszakupContext = new ParsedDirectorGoszakupContext();
            await using var webCompanyDirectorContext = new WebCompanyDirectorContext();
            await using var webCompanyDirectorContext2 = new WebCompanyDirectorContext();
            var oldest = parsedDirectorGoszakupContext.DirectorGoszakupDtos.OrderBy(x => x.Relevance).First().Relevance;
            
            // Logger.Info("Removing old information");
            // webCompanyDirectorContext.RemoveRange(webCompanyDirectorContext.CompanyDirectors.Where(x => x.RelevanceDate < oldest && x.DataSource==2));
            // webCompanyDirectorContext.SaveChanges();
            // Logger.Info("Removing ended");

            await parsedDirectorGoszakupContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.director_goszakup restart identity cascade;");
            Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("Started thread");


            await using var webCompanyDirectorContext = new WebCompanyDirectorContext();
            await using var parsedDirectorGoszakupContext = new ParsedDirectorGoszakupContext();
            await using var webCompanyContext = new WebCompanyContext();

            foreach (var dto in parsedDirectorGoszakupContext.DirectorGoszakupDtos.Where(x =>
                x.Bin != null && x.Iin != null && x.Id % NumOfThreads == threadNum))
            {
                try
                {
                    var founder = webCompanyContext.Companies.FirstOrDefault(x => x.Bin.Equals(dto.Bin) && x.FullnameDirector!=null)
                        ?.FullnameDirector;
                    if (founder != null && dto.Fullname.ToUpper().Replace(" ", string.Empty) !=
                        founder.ToUpper().Replace(" ", string.Empty))
                        continue;
                }
                catch (Exception)
                {
                    continue;
                }

                var temp = DtoToWeb(dto);
                try
                {
                    Logger.Trace($"Moving: {dto.Bin}");
                    await webCompanyDirectorContext.CompanyDirectors.Upsert(temp)
                        .On(x => x.CompanyBin).RunAsync();
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


            // Logger.Info($"Completed thread at {_total}");
        }


        private CompanyDirector DtoToWeb(DirectorGoszakupDto directorGoszakupDto)
        {
            var companyDirector = new CompanyDirector();
            companyDirector.CompanyBin = directorGoszakupDto.Bin;
            companyDirector.DirectorIin = directorGoszakupDto.Iin;
            companyDirector.DataSource = 2;
            companyDirector.RelevanceDate = directorGoszakupDto.Relevance;
            return companyDirector;
        }
    }
}