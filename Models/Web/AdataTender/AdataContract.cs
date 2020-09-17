using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
	[Table("contracts")]
	public class AdataContract
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("id_anno")] public long? IdAnno{get; set;}
		[Column("anno_number")] public string AnnoNumber{get; set;}
		[Column("contract_number")] public string ContractNumber{get; set;}
		[Column("contract_source_number")] public string ContractSourceNumber{get; set;}
		[Column("source_id")] public long? SourceId{get; set;}
		[Column("amount_sum")] public double? AmountSum{get; set;}
		[Column("biin_supplier")] public long? BiinSupplier{get; set;}
		[Column("status_id")] public long? StatusId{get; set;}
		[Column("dt_start")] public DateTime? DtStart{get; set;}
		[Column("dt_end")] public DateTime? DtEnd{get; set;}
		[Column("type_id")] public long? TypeId{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
		[Column("bin_customer")] public long? BinCustomer{get; set;}
		[Column("fin_year")] public DateTime? FinYear{get; set;}
		[Column("description_kz")] public string DescriptionKz{get; set;}
		[Column("description_ru")] public string DescriptionRu{get; set;}
		[Column("fakt_sum_wnds")] public double? FaktSumWnds{get; set;}
		[Column("sign_reason_doc_name")] public string SignReasonDocName{get; set;}
		[Column("supplier_bank_name_ru")] public string SupplierBankNameRu{get; set;}
		[Column("supplier_bank_name_kz")] public string SupplierBankNameKz{get; set;}
		[Column("customer_bank_name_ru")] public string CustomerBankNameRu{get; set;}
		[Column("customer_bank_name_kz")] public string CustomerBankNameKz{get; set;}
		[Column("method_id")] public long? MethodId{get; set;}
		
	}
	
	[Table("contract_statuses")]
	public class ContractStatuses
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
	}
	
	[Table("contract_types")]
	public class ContractTypes
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("name_kz")] public string NameKz{get; set;}
	}
}
