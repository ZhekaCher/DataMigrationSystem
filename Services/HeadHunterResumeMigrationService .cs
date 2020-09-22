using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class HeadHunterResumeMigrationService : MigrationService
    {
        private int _total;
        private int _total2;
        private readonly object _lock = new object();

        public HeadHunterResumeMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedHeadHunterContext = new ParsedHeadHunterContext();
            _total = parsedHeadHunterContext.HhResumeDtos.Count();
            _total2 = parsedHeadHunterContext.HhResumeBinDtos.Count();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webHeadHunterContext = new WebHeadHunterContext();
            await using var parsedHeadHunterContext = new ParsedHeadHunterContext();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();

            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);

            await parsedHeadHunterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.hh_resume restart identity cascade;");
            await parsedHeadHunterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.hh_resume_bin restart identity cascade;");
        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            await using var webHeadHunterContext = new WebHeadHunterContext();
            await using var parsedHeadHunterContext = new ParsedHeadHunterContext();


            foreach (var dto in parsedHeadHunterContext.HhResumeDtos.Where(x => x.Id % NumOfThreads == threadNum))
            {
                var hhResume = new HhResume
                {
                    ResumeId = dto.ResumeId,
                    Gender = dto.Gender,
                    Age = dto.Age,
                    Address = dto.Address,
                    Job = dto.Job,
                    Salary = dto.Salary,
                    GeneralExp = dto.GeneralExp,
                    WorkFor = dto.WorkFor,
                    Skills = dto.Skills,
                    RelevanceDate = dto.RelevanceDate,
                    Url = dto.Url,
                    UpdateDate = dto.UpdateDate
                };
                await webHeadHunterContext.HhResumes.Upsert(hhResume).On(x => x.ResumeId).RunAsync();
                lock (_lock)
                {
                    Logger.Trace($"Left {--_total}");
                }
            }


            foreach (var dto in parsedHeadHunterContext.HhResumeBinDtos.Where(x => x.Id % NumOfThreads == threadNum)
            )
            {
                var hhResumeBin = new HhResumeBin
                {
                    ResumeId = dto.ResumeId,
                    Bin = dto.Bin,
                    WorkPlace = dto.WorkPlace,
                    WorkInterval = dto.WorkInterval,
                    WorkDuration = dto.WorkDuration,
                    WorkPos = dto.WorkPos,
                    RelevanceDate = dto.RelevanceDate,
                    UpdateDate = dto.UpdateDate,
                    StartWork = dto.StartWork
                };

                await webHeadHunterContext.HhResumeBins.Upsert(hhResumeBin).On(x => new {x.ResumeId, x.Bin})
                    .RunAsync();
                lock (_lock)
                {
                    Logger.Trace($"Left {--_total2}");
                }
            }
        }
    }
}