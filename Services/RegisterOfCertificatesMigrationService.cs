using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    public class RegisterOfCertificatesMigrationService : MigrationService
    {
        private readonly WebRegisterOfCertificates _webRegisterOfCertificates;
        private readonly ParsedRegisterOfCertificatesContext _parsedRegisterOfCertificatesContext;
        private readonly object _forLock;
        private int _counter;

        public RegisterOfCertificatesMigrationService(int numOfThreads = 10)
        {
            _webRegisterOfCertificates = new WebRegisterOfCertificates();
            _parsedRegisterOfCertificatesContext = new ParsedRegisterOfCertificatesContext();
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
            for (var i = 0; i < 10; i++)
            {
                tasks.Add(MigrateLocal(i));
                tasks.Add(MigrateExport(i));
                tasks.Add(MigrateIndustrial(i));
            }
            await Task.WhenAll(tasks);
           
            await using var parsedRegisterOfCertificatesContext = new ParsedRegisterOfCertificatesContext();
            await parsedRegisterOfCertificatesContext.Database.ExecuteSqlRawAsync(
                "truncate avroradata.industrial_certificate, avroradata.local_certificate,avroradata.export_certificate restart identity;");

        }
        
        private async Task MigrateLocal(int numThread)
        {
            await using var webRegisterOfCertificates = new WebRegisterOfCertificates();
            await using var parsedRegisterOfCertificatesContext = new ParsedRegisterOfCertificatesContext();
            var companiesDto = parsedRegisterOfCertificatesContext.CompanyBinDtos
                .Where(x => x.Code % NumOfThreads == numThread)
                .Include(x => x.LocalCertificatesDto);

            foreach (var binDto in companiesDto)
            {
                if (binDto.LocalCertificatesDto.Count == 0)
                    continue;
                var newlist = binDto.LocalCertificatesDto.Select(x => new LocalCertificates
                {
                    BlankNum = x.BlankNum,
                    CodeRpp = x.CodeRpp,
                    RppName = x.RppName,
                    CertNum = x.CertNum,
                    Year = x.Year,
                    CertificatePurpose = x.CertificatePurpose,
                    Category = x.Category,
                    ManufacturerIinBiin = x.ManufacturerIinBiin,
                    ManufacturerName = x.ManufacturerName,
                    ManufacturerAddress = x.ManufacturerAddress,
                    GoodsName = x.GoodsName,
                    CodeTn = x.CodeTn,
                    CodeKp = x.CodeKp,
                    GoodsCount = x.GoodsCount,
                    UnitMetric = x.UnitMetric,
                    CodeOfUnitMetric = x.CodeOfUnitMetric,
                    OriginCriterion = x.OriginCriterion,
                    Percentage = x.Percentage,
                    ReceiverIinBiin = x.ReceiverIinBiin,
                    ReceiverName = x.ReceiverName,
                    ReceiverAddress = x.ReceiverAddress,
                    CertificateEndDate = x.CertificateEndDate,
                    CertificateStatus = x.CertificateStatus,
                    IssueDate = x.IssueDate,
                    RelevanceDate = DateTime.Now
                }).ToList();
                 var oldList = webRegisterOfCertificates.LocalCertificateses
                     .Where(x => x.ManufacturerIinBiin == binDto.Code).ToList();
                 webRegisterOfCertificates.LocalCertificateses.RemoveRange(oldList);
                 await webRegisterOfCertificates.SaveChangesAsync();

                await webRegisterOfCertificates.LocalCertificateses.AddRangeAsync(newlist);
                await webRegisterOfCertificates.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }

        private async Task MigrateIndustrial(int numThread)
        {
            await using var webRegisterOfCertificates = new WebRegisterOfCertificates();
            await using var parsedRegisterOfCertificatesContext = new ParsedRegisterOfCertificatesContext();
            var companiesDto = parsedRegisterOfCertificatesContext.CompanyBinDtos
                .Where(x => x.Code % NumOfThreads == numThread)
                .Include(x => x.IndustrialCertificatesDtos);
            foreach (var binDto in companiesDto)
            {
                if (binDto.IndustrialCertificatesDtos.Count == 0)
                    continue;
                var newlist = binDto.IndustrialCertificatesDtos.Select(x => new IndustrialCertificates
                {
                    IinBiin = x.IinBiin,
                    RegistrationNumber = x.RegistrationNumber,
                    ManufacturerName = x.ManufacturerName,
                    Activity = x.Activity,
                    RegionByKato = x.RegionByKato,
                    LegalAddress = x.LegalAddress,
                    StreetAddress = x.StreetAddress,
                    FactAddress = x.FactAddress,
                    Phone = x.Phone,
                    EmailAddress = x.EmailAddress,
                    Website = x.Website,
                    GoodsName = x.GoodsName,
                    GoodsCountPerYear = x.GoodsCountPerYear,
                    Tn = x.Tn,
                    Kp=x.Kp,
                    AssesmentDoc = x.AssesmentDoc,
                    IssueDate = x.IssueDate,
                    EndDate = x.EndDate,
                    LicenceNum = x.LicenceNum,
                    WorkersCount = x.WorkersCount,
                    ReesterInsertDate = x.ReesterInsertDate,
                    ChangesDate = x.ChangesDate,
                    ActualizationDate = x.ActualizationDate
                }).ToList();
                var oldList = webRegisterOfCertificates.IndustrialCertificateses
                    .Where(x => x.IinBiin == binDto.Code).ToList();
                webRegisterOfCertificates.IndustrialCertificateses.RemoveRange(oldList);
                await webRegisterOfCertificates.SaveChangesAsync();
                await webRegisterOfCertificates.IndustrialCertificateses.AddRangeAsync(newlist);
                await webRegisterOfCertificates.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }

        }

        private async Task MigrateExport(int numThread)
        {
            await using var webRegisterOfCertificates = new WebRegisterOfCertificates();
            await using var parsedRegisterOfCertificatesContext = new ParsedRegisterOfCertificatesContext();
            var companiesDto = parsedRegisterOfCertificatesContext.CompanyBinDtos
                .Where(x => x.Code % NumOfThreads == numThread)
                .Include(x => x.ExportCertificatesDtos);
            foreach (var binDto in companiesDto)
            {
                if (binDto.ExportCertificatesDtos.Count == 0)
                    continue;
                var newlist = binDto.ExportCertificatesDtos.Select(x => new ExportCertificates
                {
                    CertNum = x.CertNum,
                    RppName = x.RppName,
                    CertForm = x.CertForm,
                    BlankNum = x.BlankNum,
                    ManufacturerIinBiin = x.ManufacturerIinBiin,
                    ManufacturerName = x.ManufacturerName,
                    ManufacturerAddress = x.ManufacturerAddress,
                    GoodsName = x.GoodsName,
                    CodeTn = x.CodeTn,
                    OriginCriterion = x.OriginCriterion,
                    GoodsCountry = x.GoodsCountry,
                    ReceiverCountry = x.ReceiverCountry,
                    CertificateStatus = x.CertificateStatus,
                    IssueDate = x.IssueDate,
                    RelevanceDate = DateTime.Now

                }).ToList();
                var oldList = webRegisterOfCertificates.ExportCertificateses
                    .Where(x => x.ManufacturerIinBiin == binDto.Code).ToList();
                webRegisterOfCertificates.ExportCertificateses.RemoveRange(oldList);
                await webRegisterOfCertificates.SaveChangesAsync();
                await webRegisterOfCertificates.ExportCertificateses.AddRangeAsync(newlist);
                await webRegisterOfCertificates.SaveChangesAsync();
                lock (_forLock)
                {
                    Logger.Trace(_counter--);
                }
            }
        }
    }
}
