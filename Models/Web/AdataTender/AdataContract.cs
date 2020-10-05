using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
    [Table("contracts")]
    public class AdataContracts
    {
        [Key] [Column("id")] public long? Id { get; set; }
        [Column("source_id")] public int? SourceId { get; set; }
        [Column("announcement_number")] public string AnnouncementNumber { get; set; }
        [Column("supplier_biin")] public long? SupplierBiin { get; set; }
        [Column("customer_bin")] public long? CustomerBin { get; set; }
        [Column("supplier_iik")] public string SupplierIik { get; set; }
        [Column("customer_iik")] public string CustomerIik { get; set; }
        [Column("fin_year")] public int? FinYear { get; set; }
        [Column("sign_date")] public DateTime? SignDate { get; set; }
        [Column("ec_end_date")] public DateTime? EcEndDate { get; set; }
        [Column("description_ru")] public string DescriptionRu { get; set; }
        [Column("contract_sum_wnds")] public double? ContractSumWnds { get; set; }
        [Column("fakt_sum_wnds")] public double? FaktSumWnds { get; set; }
        [Column("supplier_bank_id")] public long? SupplierBankId { get; set; }
        [Column("customer_bank_id")] public long? CustomerBankId { get; set; }
        [Column("agr_form_id")] public long? AgrFormId { get; set; }
        [Column("year_type_id")] public long? YearTypeId { get; set; }
        [Column("method_id")] public long? MethodId { get; set; }
        [Column("status_id")] public long? StatusId { get; set; }
        [Column("type_id")] public long? TypeId { get; set; }
        [Column("source_number")] public string SourceNumber { get; set; }
        [Column("source_number_sys")] public string SourceNumberSys { get; set; }
        [Column("create_date")] public DateTime? CreateDate { get; set; }
    }

    [Table("contract_agr_forms")]
    public class ContractAgrForm
    {
        [Key] [Column("id")] public int? Id { get; set; }
        [Column("name_ru")] public string NameRu { get; set; }
    }

    [Table("contract_statuses")]
    public class ContractStatus
    {
        [Key] [Column("id")] public int? Id { get; set; }
        [Column("name_ru")] public string NameRu { get; set; }
    }

    [Table("contract_types")]
    public class ContractType
    {
        [Key] [Column("id")] public int? Id { get; set; }
        [Column("name_ru")] public string NameRu { get; set; }
    }

    [Table("contract_year_types")]
    public class ContractYearType
    {
        [Key] [Column("id")] public int? Id { get; set; }
        [Column("name_ru")] public string NameRu { get; set; }
    }
    
    [Table("banks")]
    public class Bank
    {
        [Key] [Column("id")] public int? Id{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("bik")] public string Bik{get; set;}
    }
}