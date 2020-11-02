using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationSystem.Context.Parsed;
using DataMigrationSystem.Context.Parsed.Avroradata;
using DataMigrationSystem.Context.Web.AdataTender;
using DataMigrationSystem.Context.Web.Avroradata;
using DataMigrationSystem.Context.Web.Parsing;
using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Parsed.Avroradata;
using DataMigrationSystem.Models.Web.AdataTender;
using DataMigrationSystem.Models.Web.Parsing;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace DataMigrationSystem.Services
{
    /// @author Yevgeniy Cherdantsev
    /// @date 05.10.2020 10:05:13
    /// <summary>
    /// ContractGoszakup
    /// </summary>
    public class ContractGoszakupMigrationService : MigrationService
    {
        private int _total;
        private readonly object _lock = new object();
        private const int SourceId = 2;
        private readonly List<ContractStatus> _contractStatuses;
        private readonly List<TruCode> _truCodes;
        private readonly List<Measure> _measures;
        private readonly List<Method> _methods;
        private readonly List<ContractType> _types;
        private readonly List<ContractYearType> _yearTypes;
        private readonly List<ContractAgrForm> _agrForms;
        private readonly List<Bank> _banks;

        protected override Logger InitializeLogger()
        {
            return LogManager.GetCurrentClassLogger();
        }

        public ContractGoszakupMigrationService(int numOfThreads = 20)
        {
            using var parsingContext = new ParsedContractGoszakupContext();
            _total = parsingContext.Contracts.Count();

            NumOfThreads = numOfThreads;

            using var webContractContext = new WebContractContext();

            _contractStatuses = webContractContext.ContractStatuses.ToList();
            _methods = webContractContext.Methods.ToList();
            _types = webContractContext.Types.ToList();
            _yearTypes = webContractContext.YearTypes.ToList();
            _agrForms = webContractContext.AgrForms.ToList();
            _banks = webContractContext.Banks.ToList();
            _truCodes = webContractContext.TruCodes.ToList();
            _measures = webContractContext.Measures.ToList();
        }

        public override async Task StartMigratingAsync()
        {
            Logger.Info($"Starting migration with '{NumOfThreads}' threads");
            var tasks = new List<Task>();

            await using var parsedContractGoszakupContext = new ParsedContractGoszakupContext();
            parsedContractGoszakupContext.ChangeTracker.AutoDetectChangesEnabled = false;
            foreach (var dto in parsedContractGoszakupContext.Contracts.AsNoTracking()
                .Include(x => x.Units)
                .ThenInclude(x => x.Plan)
            )
            {
                tasks.Add(Proceed(dto));
                if (tasks.Count >= NumOfThreads)
                {
                    await Task.WhenAny(tasks);

                    tasks.Where(x => x.IsFaulted).ToList().ForEach(x =>
                        Logger.Warn(x.Exception.InnerException == null
                            ? x.Exception.Message
                            : x.Exception.InnerException.Message));

                    tasks.RemoveAll(x => x.IsCompleted);
                }
            }

            await Task.WhenAll(tasks);
            Logger.Info("End of migration");

            await using var webParsingContext = new WebParsingContext();
            await webParsingContext.RelevanceDates
                .Upsert(new RelevanceDate {Code = "contracts", Relevance = DateTime.Now}).On(x => x.Code).RunAsync();
            await using var parsedGoszakupContext = new ParsedGoszakupTenderContext();
            await parsedGoszakupContext.Database.ExecuteSqlRawAsync(
                "truncate table avroradata.contract_goszakup restart identity cascade");

            Logger.Info("Successfully truncated with cascade avroradata.contract_goszakup table");
        }

        private async Task Proceed(ContractGoszakupDto dto)
        {
            await using var webContractContext = new WebContractContext();
            webContractContext.ChangeTracker.AutoDetectChangesEnabled = false;

            var webContract = DtoToWebContract(dto);
            await webContractContext.Contracts.Upsert(webContract).On(x => new {x.SourceId, x.SourceNumberSys})
                // .UpdateIf(
                //     (x, y) =>
                //         x.CustomerIik != y.CustomerIik ||
                //         x.SupplierIik != y.SupplierIik ||
                //         x.DescriptionRu != y.DescriptionRu ||
                //         x.StatusId != y.StatusId ||
                //         x.MethodId != y.MethodId ||
                //         x.TypeId != y.TypeId ||
                //         x.YearTypeId != y.YearTypeId ||
                //         x.AgrFormId != y.AgrFormId ||
                //         x.SupplierBankId != y.SupplierBankId ||
                //         x.CustomerBankId != y.CustomerBankId ||
                //         x.ContractSumWnds != y.ContractSumWnds ||
                //         x.FaktSumWnds != y.FaktSumWnds ||
                //         x.DocLink != y.DocLink ||
                //         x.DocName != y.DocName
                // )
                .RunAsync();

            if (dto.Units != null && dto.Units.Count > 0)
            {
                var webContractId = webContractContext.Contracts.FirstOrDefault(x =>
                    x.SourceId == SourceId && x.SourceNumberSys == dto.ContractNumberSys)?.Id;
                if (webContractId == null)
                    throw new NullReferenceException($"Contract with {dto.ContractNumberSys} number not found");

                foreach (var contractUnitGoszakupDto in dto.Units)
                {
                    var webUnit = DtoToWebContractUnit(contractUnitGoszakupDto);
                    webUnit.ContractId = webContractId;
                    await webContractContext.ContractUnits.Upsert(webUnit).On(x => new {x.SourceId, x.SourceUniqueId})
                        // .UpdateIf(
                        //     (x, y) =>
                        //         x.Quantity != y.Quantity ||
                        //         x.ItemPrice != y.ItemPrice ||
                        //         x.TotalSum != y.TotalSum ||
                        //         x.ItemPriceWnds != y.ItemPriceWnds ||
                        //         x.TotalSumWnds != y.TotalSumWnds
                        // )
                        .RunAsync();

                    var unitId = webContractContext.ContractUnits.FirstOrDefault(x =>
                        x.SourceId == SourceId && x.SourceUniqueId == contractUnitGoszakupDto.SourceUniqueId)?.Id;
                    if (unitId != null && contractUnitGoszakupDto.Plan != null)
                    {
                        var webPlan = DtoToWebPlan(contractUnitGoszakupDto.Plan);
                        webPlan.ContractUnitId = unitId;
                        await webContractContext.Plans.Upsert(webPlan).On(x => new {x.SourceId, x.SourceUniqueId})
                            // .UpdateIf(
                            //     (x, y) =>
                            //         x.Amount != y.Amount ||
                            //         x.Count != y.Count ||
                            //         x.ExtraDescription != y.ExtraDescription ||
                            //         x.Prepayment != y.Prepayment ||
                            //         x.Price != y.Price ||
                            //         x.ActNumber != y.ActNumber ||
                            //         x.FinYear != y.FinYear ||
                            //         x.IsQvazi != y.IsQvazi ||
                            //         x.MethodId != y.MethodId ||
                            //         x.TruCode != y.TruCode ||
                            //         x.StatusId != y.StatusId ||
                            //         x.SubjectBiin != y.SubjectBiin
                            // )
                            .RunAsync();
                    }
                }
            }

            lock (_lock)
                Logger.Trace($"Left {--_total} {Thread.CurrentThread.Name}");
        }

        private AdataContracts DtoToWebContract(ContractGoszakupDto dto)
        {
            long? methodId;
            long? statusId;
            long? agrFormId;
            long? typeId;
            long? typeYearId;
            long? customerBankId;
            long? supplierBankId;

            lock (_methods)
            {
                methodId = _methods.FirstOrDefault(x => x.Name == dto.TradeMethod)?.Id;
                if (methodId == null && dto.TradeMethod != null)
                {
                    var newWebMethod = new Method
                    {
                        Name = dto.TradeMethod
                    };
                    using var ctx = new WebContractContext();
                    ctx.Methods.Add(newWebMethod);
                    ctx.SaveChanges();
                    _methods.Add(newWebMethod);
                    methodId = newWebMethod.Id;
                }
            }

            lock (_contractStatuses)
            {
                statusId = _contractStatuses.FirstOrDefault(x => x.NameRu == dto.Status)?.Id;
                if (statusId == null && dto.Status != null)
                {
                    var newWebStatus = new ContractStatus
                    {
                        NameRu = dto.Status
                    };
                    using var ctx = new WebContractContext();
                    ctx.ContractStatuses.Add(newWebStatus);
                    ctx.SaveChanges();
                    _contractStatuses.Add(newWebStatus);
                    statusId = newWebStatus.Id;
                }
            }

            lock (_agrForms)
            {
                agrFormId = _agrForms.FirstOrDefault(x => x.NameRu == dto.AgrForm)?.Id;
                if (agrFormId == null && dto.AgrForm != null)
                {
                    var newAgrForm = new ContractAgrForm
                    {
                        NameRu = dto.AgrForm
                    };
                    using var ctx = new WebContractContext();
                    ctx.AgrForms.Add(newAgrForm);
                    ctx.SaveChanges();
                    _agrForms.Add(newAgrForm);
                    agrFormId = newAgrForm.Id;
                }
            }

            lock (_types)
            {
                typeId = _types.FirstOrDefault(x => x.NameRu == dto.Type)?.Id;
                if (typeId == null && dto.Type != null)
                {
                    var newType = new ContractType
                    {
                        NameRu = dto.Type
                    };
                    using var ctx = new WebContractContext();
                    ctx.Types.Add(newType);
                    ctx.SaveChanges();
                    _types.Add(newType);
                    typeId = newType.Id;
                }
            }

            lock (_yearTypes)
            {
                typeYearId = _yearTypes.FirstOrDefault(x => x.NameRu == dto.YearType)?.Id;
                if (typeYearId == null && dto.YearType != null)
                {
                    var newYearType = new ContractYearType
                    {
                        NameRu = dto.YearType
                    };
                    using var ctx = new WebContractContext();
                    ctx.YearTypes.Add(newYearType);
                    ctx.SaveChanges();
                    _yearTypes.Add(newYearType);
                    typeYearId = newYearType.Id;
                }
            }

            lock (_banks)
            {
                customerBankId = _banks
                    .FirstOrDefault(x => x.NameRu == dto.CustomerBankNameRu && x.Bik == dto.CustomerBik)?.Id;
                if (customerBankId == null && (dto.CustomerBankNameRu != null || dto.CustomerBik != null))
                {
                    var newBank = new Bank
                    {
                        NameRu = dto.CustomerBankNameRu,
                        Bik = dto.CustomerBik
                    };
                    using var ctx = new WebContractContext();
                    ctx.Banks.Add(newBank);
                    ctx.SaveChanges();
                    _banks.Add(newBank);
                    customerBankId = newBank.Id;
                }

                supplierBankId = _banks
                    .FirstOrDefault(x => x.NameRu == dto.SupplierBankNameRu && x.Bik == dto.SupplierBik)?.Id;
                if (supplierBankId == null && (dto.SupplierBankNameRu != null || dto.SupplierBik != null))
                {
                    var newBank = new Bank
                    {
                        NameRu = dto.SupplierBankNameRu,
                        Bik = dto.SupplierBik
                    };
                    using var ctx = new WebContractContext();
                    ctx.Banks.Add(newBank);
                    ctx.SaveChanges();
                    _banks.Add(newBank);
                    supplierBankId = newBank.Id;
                }
            }

            return new AdataContracts
            {
                AnnouncementNumber = dto.AnnouncementNumber,
                CreateDate = dto.CreateDate,
                CustomerBin = dto.CustomerBin,
                CustomerIik = dto.CustomerIik,
                DescriptionRu = dto.DescriptionRu,
                FinYear = dto.FinYear,
                MethodId = methodId,
                SignDate = dto.SignDate,
                SourceId = SourceId,
                SourceNumber = dto.ContractNumber,
                StatusId = statusId,
                SupplierBiin = dto.SupplierBiin,
                SupplierIik = dto.SupplierIik,
                TypeId = typeId,
                AgrFormId = agrFormId,
                ContractSumWnds = dto.ContractSumWnds,
                CustomerBankId = customerBankId,
                EcEndDate = dto.EcEndDate,
                FaktSumWnds = dto.FaktSumWnds,
                SourceNumberSys = dto.ContractNumberSys,
                SupplierBankId = supplierBankId,
                YearTypeId = typeYearId,
                DocLink = dto.DocLink,
                DocName = dto.DocName
            };
        }

        private ContractUnit DtoToWebContractUnit(ContractUnitGoszakupDto dto)
        {
            return new ContractUnit
            {
                ItemPrice = dto.ItemPrice,
                Quantity = dto.Quantity,
                TotalSum = dto.TotalSum,
                ItemPriceWnds = dto.ItemPriceWnds,
                TotalSumWnds = dto.TotalSumWnds,
                SourceUniqueId = dto.SourceUniqueId,
                SourceId = SourceId
            };
        }

        private AdataPlan DtoToWebPlan(PlanGoszakupDto dto)
        {
            long? methodId;
            long? statusId;
            long? measureId;
            string truCode;

            lock (_methods)
            {
                methodId = _methods.FirstOrDefault(x => x.Name == dto.Method)?.Id;
                if (methodId == null && dto.Method != null)
                {
                    var newWebMethod = new Method
                    {
                        Name = dto.Method
                    };
                    using var ctx = new WebContractContext();
                    ctx.Methods.Add(newWebMethod);
                    ctx.SaveChanges();
                    _methods.Add(newWebMethod);
                    methodId = newWebMethod.Id;
                }
            }

            lock (_contractStatuses)
            {
                statusId = _contractStatuses.FirstOrDefault(x => x.NameRu == dto.Status)?.Id;
                if (statusId == null && dto.Status != null)
                {
                    var newWebStatus = new ContractStatus
                    {
                        NameRu = dto.Status
                    };
                    using var ctx = new WebContractContext();
                    ctx.ContractStatuses.Add(newWebStatus);
                    ctx.SaveChanges();
                    _contractStatuses.Add(newWebStatus);
                    statusId = newWebStatus.Id;
                }
            }

            lock (_measures)
            {
                measureId = _measures.FirstOrDefault(x => x.Name == dto.Measure)?.Id;
                if (measureId == null && dto.Measure != null)
                {
                    var newWebMeasure = new Measure
                    {
                        Name = dto.Measure
                    };
                    using var ctx = new WebContractContext();
                    ctx.Measures.Add(newWebMeasure);
                    ctx.SaveChanges();
                    _measures.Add(newWebMeasure);
                    measureId = newWebMeasure.Id;
                }
            }

            lock (_truCodes)
            {
                truCode = _truCodes.FirstOrDefault(x => x.Code == dto.TruCode)?.Code;
                if (truCode == null && dto.TruCode != null)
                {
                    var newTruCode = new TruCode
                    {
                        Code = dto.TruCode,
                        Name = dto.Name,
                        Characteristics = dto.Description
                    };
                    using var ctx = new WebContractContext();
                    ctx.TruCodes.Add(newTruCode);
                    ctx.SaveChanges();
                    _truCodes.Add(newTruCode);
                }
            }

            return new AdataPlan
            {
                Amount = dto.Amount,
                Count = dto.Count,
                Description = dto.Description,
                Name = dto.Name,
                Prepayment = dto.Prepayment,
                Price = dto.Price,
                ActNumber = dto.ActNumber,
                DateApproved = dto.DateApproved,
                DateCreate = dto.DateCreate,
                ExtraDescription = dto.ExtraDescription,
                FinYear = dto.FinYear,
                IsQvazi = dto.IsQvazi,
                MeasureId = measureId,
                MethodId = methodId,
                MonthId = dto.MonthId,
                SourceId = SourceId,
                StatusId = statusId,
                SubjectBiin = dto.SubjectBiin,
                SupplyDate = dto.SupplyDate,
                TruCode = dto.TruCode,
                SourceUniqueId = dto.SourceUniqueId
            };
        }
    }
}