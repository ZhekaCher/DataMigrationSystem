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
    public class OtherParticipantsMigrationService : MigrationService
    {
        public OtherParticipantsMigrationService(int numOfThreads = 10)
        {
            NumOfThreads = numOfThreads;
            using var parsedOtherParticipantsContext = new ParsedOtherParticipantsContext();
           

        }
        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            await using var webOtherParticipantsContext = new WebOtherParticipantsContext();
            await using var parsedOtherParticipantsContext = new ParsedOtherParticipantsContext();
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));
            await Task.WhenAll(tasks);
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.accountants restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.affiliated_persons restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.board_of_directors restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.exchange_docs restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.executive_agency restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.main_info restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.org_types restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.relations_by_contacts restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.relations_by_reports restart identity cascade;");
            await parsedOtherParticipantsContext.Database.ExecuteSqlRawAsync("truncate table avroradata.shareholders restart identity cascade;");
            Logger.Info("Truncated");
            Logger.Info("End of migration");
        }

        private  async Task Migrate(int threadNum)
        {
            Logger.Info("started thread");
            
            await using var webOtherParticipantsContext = new WebOtherParticipantsContext();
            await using var parsedOtherParticipantsContext = new ParsedOtherParticipantsContext();
            var otherParticipants = from dto in parsedOtherParticipantsContext.OtherParticipantDtos
                where dto.Id % NumOfThreads == threadNum
                select new OtherParticipant
                {
                    CodeBin = dto.CodeBin,
                    FirstDirector = dto.FirstDirector,
                    Telephone = dto.Telephone,
                    Mail = dto.Mail,
                    WebSite = dto.WebSite,
                    LinkId = dto.LinkId
                    
                };
            await webOtherParticipantsContext.MainInfos.UpsertRange(otherParticipants).On(x => x.CodeBin).RunAsync();
            var exchangeDocs = from dto in parsedOtherParticipantsContext.ExchangeDocsDtos
                where dto.Id % NumOfThreads == threadNum
                select new ExchangeDoc
                {
                    CompId = dto.CompId,
                    CodeCb = dto.CodeCb,
                    Isin = dto.Isin,
                    Nin = dto.Nin,
                    CbType = dto.CbType,
                    IncludeDate = dto.IncludeDate,
                    ExcludeDate = dto.ExcludeDate
                    
                };
            await webOtherParticipantsContext.ExchangeDocs.UpsertRange(exchangeDocs).On(x => x.CodeCb).RunAsync();

            var affiliatedPersons = from dto in parsedOtherParticipantsContext.AffiliatedPersonsDtos
                where dto.Id % NumOfThreads == threadNum
                select new AffiliatedPerson
                {
                    CompId = dto.CompId,
                    IsIndividual = dto.IsIndividual,
                    FullName = dto.FullName,
                    AffiliationDate = dto.AffiliationDate,
                    BirthDate = dto.BirthDate,
                    RegDate = dto.RegDate,
                    RegNum = dto.RegNum,
                    BasisForAffiliation = dto.BasisForAffiliation,
                    ReportDate = dto.ReportDate,
                    Biin = dto.Biin
                    
                };
            await webOtherParticipantsContext.AffiliatedPersons.UpsertRange(affiliatedPersons).On(x => new {x.CompId, x.FullName}).RunAsync();
            var boardOfDirectors = from dto in parsedOtherParticipantsContext.BoardOfDirectorsDtos
                where dto.Id % NumOfThreads == threadNum
                select new BoardOfDirector
                {
                    CompId = dto.CompId,
                    DecisionNumber = dto.DecisionNumber,
                    FullName = dto.FullName,
                    DecisionDate = dto.DecisionDate,
                    ReportDate = dto.ReportDate,
                    Iin = dto.Iin
                    
                };
            await webOtherParticipantsContext.BoardOfDirectors.UpsertRange(boardOfDirectors).On(x => new {x.CompId, x.FullName}).RunAsync();
            var shareholders = from dto in parsedOtherParticipantsContext.ShareholdersDtos
                where dto.Id % NumOfThreads == threadNum
                select new Shareholder
                {
                    CompId = dto.CompId,
                    IsIndividual = dto.IsIndividual , 
                    FullName = dto.FullName,
                    IsResident = dto.IsResident,
                    Share = dto.Share,
                    ReportDate = dto.ReportDate,
                    Biin = dto.Biin
                };
            await webOtherParticipantsContext.Shareholders.UpsertRange(shareholders).On(x => new {x.CompId, x.FullName}).RunAsync();
            var orgTypes = from dto in parsedOtherParticipantsContext.OrgTypesDtos
                where dto.Id % NumOfThreads == threadNum
                select new OrgType
                {
                    CompId = dto.CompId,
                    Type = dto.Type,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };
            await webOtherParticipantsContext.OrgTypes.UpsertRange(orgTypes).On(x => new {x.CompId, x.Type}).RunAsync();
            var relationByReports = from dto in parsedOtherParticipantsContext.RelationByReportsDtos
                where dto.Id % NumOfThreads == threadNum
                select new RelationByReport
                {
                    CompId = dto.CompId,
                    TiedComp = dto.TiedComp,
                    RelationLink = dto.RelationLink,
                    RelationType = dto.RelationType,
                    Type = dto.Type,
                    Year = dto.Year,
                    UploadDate = dto.UploadDate,
                    Section = dto.Section,
                    FullName = dto.FullName
                };
            await webOtherParticipantsContext.RelationByReports.UpsertRange(relationByReports).On(x => new {x.CompId, x.TiedComp, x.RelationLink}).RunAsync();
            var relationByContacts = from dto in parsedOtherParticipantsContext.RelationByContactsDtos
                where dto.Id % NumOfThreads == threadNum
                select new RelationByContact
                {
                    CompId = dto.CompId,
                    TiedComp = dto.TiedComp,
                    RelationLink = dto.RelationLink,
                    RelationType = dto.RelationType,
                    FullName = dto.FullName
                };
            await webOtherParticipantsContext.RelationByContacts.UpsertRange(relationByContacts).On(x => new {x.CompId, x.TiedComp, x.RelationLink}).RunAsync();
            var executors = from dto in parsedOtherParticipantsContext.ExecutorsDtos
                where dto.Id % NumOfThreads == threadNum
                select new Executor
                {
                    CompId = dto.CompId,
                    decisionNumber = dto.decisionNumber,
                    FullName = dto.FullName,
                    DecisionDate = dto.DecisionDate,
                    ReportDate = dto.ReportDate,
                    Iin = dto.Iin
                };
            await webOtherParticipantsContext.Executors.UpsertRange(executors).On(x => new {x.CompId, x.FullName}).RunAsync();
            var acccountants = from dto in parsedOtherParticipantsContext.AcccountantsDtos
                where dto.Id % NumOfThreads == threadNum
                select new Acccountant
                {
                    CompId = dto.CompId,
                    FullName = dto.FullName,
                    Certifier = dto.Certifier,
                    CertificateNumber = dto.CertificateNumber,
                    CertificationDate = dto.CertificationDate,
                    AccountantOrganization = dto.AccountantOrganization,
                    MembershipCard = dto.MembershipCard,
                    JoinDate = dto.JoinDate,
                    Iin = dto.Iin
                };
            await webOtherParticipantsContext.Acccountants.UpsertRange(acccountants).On(x => x.CompId).RunAsync();

        }
    }
}