using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("contract_goszakup")]
	public class ContractGoszakupDto
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("announcement_number")] public string AnnouncementNumber{get; set;}
		[Column("contract_number")] public string ContractNumber{get; set;}
		[Column("contract_number_sys")] public string ContractNumberSys{get; set;}
		[Column("supplier_biin")] public long? SupplierBiin{get; set;}
		[Column("customer_bin")] public long? CustomerBin{get; set;}
		[Column("supplier_iik")] public string SupplierIik{get; set;}
		[Column("customer_iik")] public string CustomerIik{get; set;}
		[Column("fin_year")] public int? FinYear{get; set;}
		[Column("sign_date")] public DateTime? SignDate{get; set;}
		[Column("create_date")] public DateTime? CreateDate{get; set;}
		[Column("ec_end_date")] public DateTime? EcEndDate{get; set;}
		[Column("description_ru")] public string DescriptionRu{get; set;}
		[Column("contract_sum_wnds")] public double? ContractSumWnds{get; set;}
		[Column("fakt_sum_wnds")] public double? FaktSumWnds{get; set;}
		[Column("supplier_bank_name_ru")] public string SupplierBankNameRu{get; set;}
		[Column("customer_bank_name_ru")] public string CustomerBankNameRu{get; set;}
		[Column("supplier_bik")] public string SupplierBik{get; set;}
		[Column("customer_bik")] public string CustomerBik{get; set;}
		[Column("agr_form")] public string AgrForm{get; set;}
		[Column("year_type")] public string YearType{get; set;}
		[Column("trade_method")] public string TradeMethod{get; set;}
		[Column("status")] public string Status{get; set;}
		[Column("type")] public string Type{get; set;}
		[Column("doc_link")] public string DocLink{get; set;}
		[Column("doc_name")] public string DocName{get; set;}
		public List<ContractUnitGoszakupDto> Units { get; set; }
	}
}
