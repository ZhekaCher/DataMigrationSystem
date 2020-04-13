using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class HeadHunterMigrationService : MigrationService
    {
        private int _total;
        private int _total2;
        private readonly object _lock = new object();

        public HeadHunterMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedHeadHunterContext = new ParsedHeadHunterContext();
            _total = parsedHeadHunterContext.CompanyHhDtos.Count();
            _total2 = parsedHeadHunterContext.VacancyHhDtos.Count();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webHeadHunterContext = new WebHeadHunterContext();
            await using var parsedHeadHunterContext = new ParsedHeadHunterContext();
            DateTime? starDate = await parsedHeadHunterContext.VacancyHhDtos.MinAsync(x => x.RelevanceDate);
            DateTime startDate;
            try
            {
                 startDate = DateTime.ParseExact(starDate.ToString(), "dd.MM.yyyy hh:mm:ss",
                    CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                startDate = DateTime.ParseExact(starDate.ToString(), "dd.MM.yyyy h:mm:ss",
                    CultureInfo.InvariantCulture);
            }
            var date = startDate.ToString("yyyy-MM-dd hh:mm:ss");
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (int i = 0; i < NumOfThreads; i++)
            {
                tasks.Add(Migrate(i));
            }

            await Task.WhenAll(tasks);
            await webHeadHunterContext.Database.ExecuteSqlRawAsync(
                $"update avroradata.hh_vacancies set active = false where relevance_date <'{date}';");
            await parsedHeadHunterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.hh_companies restart identity cascade;");
            await parsedHeadHunterContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.hh_vacancies restart identity cascade;");

        }

        private async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            await using var webHeadHunterContext = new WebHeadHunterContext();
            await using var parsedHeadHunterContext = new ParsedHeadHunterContext();
            foreach (var dto in parsedHeadHunterContext.CompanyHhDtos.Where(x=>x.Id% NumOfThreads == threadNum))
            {
                var comp = CompdtoToWeb(dto);
                await webHeadHunterContext.CompanyHhs.Upsert(comp).On(x => x.CompId).RunAsync();
                lock (_lock)
                {
                    Logger.Trace($"Left {--_total}");
                }
            }

            foreach (var dto in parsedHeadHunterContext.VacancyHhDtos.Where(x=>x.Id% NumOfThreads == threadNum))
            {
                var vac = VacDtoToWeb(dto);
                await webHeadHunterContext.VacancyHhs.Upsert(vac).On(x => x.VacId).RunAsync();
                lock (_lock)
                {
                    Logger.Trace($"Left {--_total2}");
                }
            }
        }

        private CompanyHh CompdtoToWeb(CompanyHhDto companyHhDto)
        {
            var companyHh= new CompanyHh();
            companyHh.Name = companyHhDto.Name;
            companyHh.CompId = companyHhDto.CompId;
            companyHh.Activities = companyHhDto.Activities;
            companyHh.Description = companyHhDto.Description;
            companyHh.Region = companyHhDto.Region;
            companyHh.WebSite = companyHhDto.WebSite;
            companyHh.Logo = companyHhDto.Logo;
            return companyHh;
        }

        private VacancyHh VacDtoToWeb(VacancyHhDto vacancyHhDto)
        {
            var vacancyHh = new VacancyHh();
            vacancyHh.VacTitle = vacancyHhDto.VacTitle;
            vacancyHh.Salary = vacancyHhDto.Salary;
            vacancyHh.Region = vacancyHhDto.Region;
            vacancyHh.Description = vacancyHhDto.Description;
            vacancyHh.ContactsFio = vacancyHhDto.ContactsFio;
            vacancyHh.CreateTime = vacancyHhDto.CreateTime;
            vacancyHh.VacId = vacancyHhDto.VacId;
            vacancyHh.ContactsPhone = vacancyHhDto.ContactsPhone;
            vacancyHh.ContactsEmail = vacancyHhDto.ContactsEmail;
            vacancyHh.RelevanceDate = vacancyHhDto.RelevanceDate;
            vacancyHh.Active = vacancyHhDto.Active;
            vacancyHh.CompId = vacancyHhDto.CompId;
            vacancyHh.Skills = vacancyHhDto.Skills;
            vacancyHh.Experience = vacancyHhDto.Experience;
            vacancyHh.Employment = vacancyHhDto.Employment;
            vacancyHh.Source = vacancyHhDto.Source;
            return vacancyHh;
        }
    }
}