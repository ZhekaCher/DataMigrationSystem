using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 24.02.2020 18:23:38
    /// @version 1.0
    /// <summary>
    /// ContractGoszakup
    /// </summary>
    public class ContractGoszakupMigrationService : MigrationService
    {
        private int _total;
        private object _lock = new object();
        private List<ContractStatuses> _contractStatuses;
        private List<ContractTypes> _contractTypes;
        private List<Method> _contractMethods;

        public ContractGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            _total = parsedContractGoszakupContext.ContractGoszakupDtos.Count();
            parsedContractGoszakupContext.Dispose();
            using var webContractContext = new WebContractContext();
            _contractMethods = webContractContext.ContractMethods.ToList();
            _contractStatuses = webContractContext.ContractStatuses.ToList();
            _contractTypes = webContractContext.ContractTypes.ToList();
            webContractContext.Dispose();
        }

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();
            for (var i = 0; i < NumOfThreads; i++)
                tasks.Add(Migrate(i));

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");
            // await using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            // await parsedContractGoszakupContext.Database.ExecuteSqlRawAsync(
            // "truncate table avroradata.contract_goszakup restart identity cascade;");
            // Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            // Logger.Info("Started thread");


            await using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            foreach (var dto in parsedContractGoszakupContext.ContractGoszakupDtos.AsNoTracking().Where(x =>
                    x.Id % NumOfThreads == threadNum)
                .Include(x => x.RefStatus)
                .Include(x => x.RefType)
                .Include(x => x.RefTradeMethod)
            )
            {
                var contract = DtoToWeb(dto);
                try
                {
                    await using var webContractContext = new WebContractContext();
                    webContractContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    await webContractContext.Contracts.Upsert(contract)
                        .On(x => new {x.SourceId, x.ContractSourceNumber}).RunAsync();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("violates foreign key"))
                    {
                        // Logger.Warn($"Message:|{e.Message}|; IdContract:|{temp.IdContract}|;");
                    }
                    else
                    {
                        Logger.Error(
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; IdContract:|{contract.ContractSourceNumber}|; Id:|{contract}|");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            // Logger.Info($"Completed thread at {_total}");
        }

        private AdataContract DtoToWeb(ContractGoszakupDto dto)
        {
            var contract = new AdataContract
            {
                ContractSourceId = dto.Id,
                AmountSum = dto.ContractSumWnds,
                AnnoNumber = dto.TrdBuyNumberAnno,
                BiinSupplier = dto.SupplierBiin,
                BinCustomer = dto.CustomerBin,
                ContractNumber = dto.ContractNumber,
                DescriptionKz = dto.DescriptionKz,
                DescriptionRu = dto.DescriptionRu,
                DtEnd = dto.EcEndDate,
                DtStart = dto.SignDate,
                FinYear = dto.FinYear,
                RelevanceDate = dto.Relevance,
                SourceId = 2,
                ContractSourceNumber = dto.ContractNumberSys,
                FaktSumWnds = dto.FaktSumWnds,
                CustomerBankNameKz = dto.CustomerBankNameKz,
                CustomerBankNameRu = dto.CustomerBankNameRu,
                SupplierBankNameKz = dto.SupplierBankNameKz,
                SupplierBankNameRu = dto.SupplierBankNameRu,
                SignReasonDocName = dto.SignReasonDocName
            };
            if (dto.RefStatus != null)
                contract.StatusId =
                    _contractStatuses.FirstOrDefault(x => x.NameRu == dto.RefStatus.NameRu)?.Id;
            if (dto.RefTradeMethod != null)
                contract.MethodId =
                    _contractMethods.FirstOrDefault(x => x.Name == dto.RefTradeMethod.NameRu)?.Id;
            if (dto.RefStatus != null)
                contract.TypeId =
                    _contractTypes.FirstOrDefault(x => x.NameRu == dto.RefType.NameRu)?.Id;
            var annoCtx = new WebTenderContext();
            contract.IdAnno = annoCtx.AdataAnnouncements.FirstOrDefault(x => x.SourceNumber == dto.ContractNumber)?.Id;
            annoCtx.Dispose();


            // contract.AmountSum = contractGoszakupDto.ContractSumWnds;
            // contract.BinCustomer = contractGoszakupDto.CustomerBin;
            // contract.BiinSupplier = contractGoszakupDto.SupplierBiin;
            // contract.DtEnd = contractGoszakupDto.EcEndDate;
            // contract.DtStart = contractGoszakupDto.SignDate;
            // contract.FinYear = contractGoszakupDto.FinYear;
            // contract.IdAnno = contractGoszakupDto.TrdBuyId;
            // contract.IdContract = contractGoszakupDto.Id;
            // contract.IdStatus = contractGoszakupDto.RefContractStatusId;
            // contract.IdType = contractGoszakupDto.RefContractTypeId;
            // contract.NumberContract = contractGoszakupDto.ContractNumberSys;
            // TODO(Relevance loading from dto incorrect)
            // contract.RelevanceDate = contractGoszakupDto.Relevance;
            // contractGoszakup.DescriptionKz = contractGoszakupDto.DescriptionKz;
            // contractGoszakup.DescriptionRu = contractGoszakupDto.DescriptionRu;
            // contractGoszakup.IdContract = contractGoszakupDto.Id;
            // contractGoszakup.FaktSumWnds = contractGoszakupDto.FaktSumWnds;
            // contractGoszakup.CustomerBankNameKz = contractGoszakupDto.CustomerBankNameKz;
            // contractGoszakup.CustomerBankNameRu = contractGoszakupDto.CustomerBankNameRu;
            // contractGoszakup.SupplierBankNameKz = contractGoszakupDto.SupplierBankNameKz;
            // contractGoszakup.SupplierBankNameRu = contractGoszakupDto.SupplierBankNameRu;
            // contractGoszakup.SignReasonDocName = contractGoszakupDto.SignReasonDocName;
            // contractGoszakup.TypeContract = contractGoszakupDto.RefContractTypeId;
            // contractGoszakup.TradeMethod = contractGoszakupDto.FaktTradeMethodsId;
            // return (contract, contractGoszakup);
            return contract;
        }
    }
}