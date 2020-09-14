using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
	[Table("contract")]
	public class Contract
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("id_anno")] public long? IdAnno{get; set;}
		[Column("id_contract")] public long? IdContract{get; set;}
		[Column("number_contract")] public string NumberContract{get; set;}
		[Column("id_tf")] public long? IdTf{get; set;}
		[Column("amount_sum")] public double? AmountSum{get; set;}
		[Column("biin_supplier")] public long? BiinSupplier{get; set;}
		[Column("id_status")] public long? IdStatus{get; set;}
		[Column("dt_start")] public DateTime? DtStart{get; set;}
		[Column("dt_end")] public DateTime? DtEnd{get; set;}
		[Column("id_type")] public long? IdType{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
		// [Column("add_info")] public string AddInfo{get; set;}
		[Column("bin_customer")] public long? BinCustomer{get; set;}
		[Column("fin_year")] public DateTime? FinYear{get; set;}
		
	}
}
