using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
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
    public class PlanGoszakupMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public PlanGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedPlanGoszakupContext = new ParsedPlanGoszakupContext();
            _total = parsedPlanGoszakupContext.PlanGoszakupDtos.Count();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            // await parsedParticipantGoszakupContext.Database.ExecuteSqlRawAsync("truncate table avroradata.participant_goszakup restart identity cascade;");
            // Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            await using var webPlanGoszakupContext = new WebPlanContext();
            await using var parsedPlanGoszakupContext = new ParsedPlanGoszakupContext();
            webPlanGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;
            parsedPlanGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;
            foreach (var dto in parsedPlanGoszakupContext.PlanGoszakupDtos.AsNoTracking().Take(20).Where(x =>
                    x.Id % NumOfThreads == threadNum))
            {
                var temp = DtoToWeb(dto);
                try
                {
                    await webPlanGoszakupContext.Plans.Upsert(temp)
                        .On(x => x.Id).RunAsync();
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
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; Biin:|{temp.SubjectBiin}|; Id:|{temp.Id}|");
                        Program.NumOfErrors++;
                    }
                }
                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            // Logger.Info($"Completed thread at {_total}");
        }


        private Plan DtoToWeb(PlanGoszakupDto planGoszakupDto)
        {
            var planGoszakup = new Plan
            {
                PlanSourceId = planGoszakupDto.Id,
                Amount =  planGoszakupDto.Amount,
                Count = planGoszakupDto.Count,
                Prepayment = planGoszakupDto.Prepayment,
                Price = planGoszakupDto.Prepayment,
                Sum1 = planGoszakupDto.Sum1,
                Sum2 = planGoszakupDto.Sum2,
                Sum3 = planGoszakupDto.Sum3,
                Timestamp = planGoszakupDto.Timestamp,
                DateApproved = planGoszakupDto.DateApproved,
                DateCreate = planGoszakupDto.DateCreate,
                DescKz = planGoszakupDto.DescKz,
                DescRu = planGoszakupDto.DescRu,
                IsQvazi = planGoszakupDto.IsQvazi,
                NameKz = planGoszakupDto.NameKz,
                NameRu = planGoszakupDto.NameRu,
                PlanPreliminary = planGoszakupDto.PlanPreliminary,
                RootrecordId = planGoszakupDto.RootrecordId,
                SubjectBiin = planGoszakupDto.SubjectBiin,
                SystemId = planGoszakupDto.SystemId,
                TransferType = planGoszakupDto.TransferType,
                DisablePersonId = planGoszakupDto.DisablePersonId,
                ExtraDescKz = planGoszakupDto.ExtraDescKz,
                ExtraDescRu = planGoszakupDto.ExtraDescRu,
                PlanActId = planGoszakupDto.PlanActId,
                PlanActNumber = planGoszakupDto.PlanActNumber,
                PlanFinYear = planGoszakupDto.PlanFinYear,
                PlnPointYear = planGoszakupDto.PlnPointYear,
                RefAbpCode = planGoszakupDto.RefAbpCode,
                RefEnstruCode = planGoszakupDto.RefEnstruCode,
                RefFinsourceId = planGoszakupDto.RefFinsourceId,
                RefJustificationId = planGoszakupDto.RefJustificationId,
                RefMonthsId = planGoszakupDto.RefMonthsId,
                RefUnitsCode = planGoszakupDto.RefUnitsCode,
                SubjectNameKz = planGoszakupDto.SubjectNameKz,
                SubjectNameRu = planGoszakupDto.SubjectNameRu,
                SupplyDateRu = planGoszakupDto.SupplyDateRu,
                SysSubjectsId = planGoszakupDto.SysSubjectsId,
                ContractPrevPointId = planGoszakupDto.ContractPrevPointId,
                RefBudgetTypeId = planGoszakupDto.RefBudgetTypeId,
                RefPlanStatusId = planGoszakupDto.RefPlanStatusId,
                RefPointTypeId = planGoszakupDto.RefPointTypeId,
                RefSubjectTypesId = planGoszakupDto.RefSubjectTypesId,
                RefTradeMethodsId = planGoszakupDto.RefTradeMethodsId,
                TransferSysSubjectsId = planGoszakupDto.TransferSysSubjectsId,
                RefAmendmAgreemJustifId = planGoszakupDto.RefAmendmAgreemJustifId,
                RefAmendmentAgreemTypeId = planGoszakupDto.RefAmendmentAgreemTypeId,
                RefPlnPointStatusId = planGoszakupDto.RefPlnPointStatusId
            };
            return planGoszakup;
        }
    }
}