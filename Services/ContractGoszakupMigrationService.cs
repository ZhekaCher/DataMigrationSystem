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
    /// INPUT
    /// </summary>
    public class ContractGoszakupMigrationService : MigrationService
    {
        private const string CurrentTradingFloor = "goszakup";
        private readonly int _sTradingFloorId;
        private int _total;
        private readonly object _lock = new object();

        public ContractGoszakupMigrationService(int numOfThreads = 30)
        {
            NumOfThreads = numOfThreads;
            using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            using var webContractContext = new WebContractContext();
            _total = parsedContractGoszakupContext.ContractGoszakupDtos.Count();
            _sTradingFloorId = webContractContext.STradingFloors
                .FirstOrDefault(x => x.Code.Equals(CurrentTradingFloor)).Id;
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
            await using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            await parsedContractGoszakupContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.contract_goszakup restart identity cascade;");
            Logger.Info("Truncated");
        }

        private async Task Migrate(int threadNum)
        {
            // Logger.Info("Started thread");


            await using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            foreach (var dto in parsedContractGoszakupContext.ContractGoszakupDtos.AsNoTracking().Where(x =>
                x.Id % NumOfThreads == threadNum))
            {
                Contract temp;
                ContractGoszakup tempGoszakup;
                (temp, tempGoszakup) = DtoToWeb(dto);
                temp.IdTf = _sTradingFloorId;
                try
                {
                    await using var webContractContext = new WebContractContext();
                    webContractContext.ChangeTracker.AutoDetectChangesEnabled = false;
                    await webContractContext.Contracts.Upsert(temp)
                        .On(x => new {x.IdContract, x.IdTf}).RunAsync();
                    await webContractContext.ContractsGoszakup.Upsert(tempGoszakup)
                        .On(x => x.IdContract).RunAsync();
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
                            $"Message:|{e.Message}|; StackTrace:|{e.StackTrace}|; IdContract:|{temp.IdContract}|; Id:|{temp.Id}|");
                        Program.NumOfErrors++;
                    }
                }

                lock (_lock)
                    Logger.Trace($"Left {--_total}");
            }


            // Logger.Info($"Completed thread at {_total}");
        }

        private (Contract, ContractGoszakup) DtoToWeb(ContractGoszakupDto contractGoszakupDto)
        {
            var contract = new Contract();
            var contractGoszakup = new ContractGoszakup();

            contract.AmountSum = contractGoszakupDto.ContractSumWnds;
            contract.BinCustomer = contractGoszakupDto.CustomerBin;
            contract.BiinSupplier = contractGoszakupDto.SupplierBiin;
            contract.DtEnd = contractGoszakupDto.EcEndDate;
            contract.DtStart = contractGoszakupDto.SignDate;
            contract.FinYear = contractGoszakupDto.FinYear;
            contract.IdAnno = contractGoszakupDto.TrdBuyId;
            contract.IdContract = contractGoszakupDto.Id;
            contract.IdStatus = contractGoszakupDto.RefContractStatusId;
            contract.IdType = contractGoszakupDto.RefContractTypeId;
            contract.NumberContract = contractGoszakupDto.ContractNumberSys;
            //TODO(Relevance loading from dto incorrect)
            contract.RelevanceDate = contractGoszakupDto.Relevance;
            contractGoszakup.DescriptionKz = contractGoszakupDto.DescriptionKz;
            contractGoszakup.DescriptionRu = contractGoszakupDto.DescriptionRu;
            contractGoszakup.IdContract = contractGoszakupDto.Id;
            contractGoszakup.FaktSumWnds = contractGoszakupDto.FaktSumWnds;
            contractGoszakup.CustomerBankNameKz = contractGoszakupDto.CustomerBankNameKz;
            contractGoszakup.CustomerBankNameRu = contractGoszakupDto.CustomerBankNameRu;
            contractGoszakup.SupplierBankNameKz = contractGoszakupDto.SupplierBankNameKz;
            contractGoszakup.SupplierBankNameRu = contractGoszakupDto.SupplierBankNameRu;
            contractGoszakup.SignReasonDocName = contractGoszakupDto.SignReasonDocName;
            contractGoszakup.TypeContract = contractGoszakupDto.RefContractTypeId;
            contractGoszakup.TradeMethod = contractGoszakupDto.FaktTradeMethodsId;
            return (contract, contractGoszakup);
        }
    }
}