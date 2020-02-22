using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
	[Table("unscrupulouses_goszakup")]
	public class UnscrupulousesGoszakupDto
	{
		[Key] [Column("pid")] public int? Pid{get; set;}
		[Column("supplier_biin")] public long? SupplierBiin{get; set;}
		[Column("supplier_innunp")] public string SupplierInnunp{get; set;}
		[Column("supplier_name_ru")] public string SupplierNameRu{get; set;}
		[Column("supplier_name_kz")] public string SupplierNameKz{get; set;}
		[Column("index_date")] public DateTime? IndexDate{get; set;}
		[Column("system_id")] public int? SystemId{get; set;}
		[Column("relevance")] public DateTime? Relevance{get; set;}
		
	}
}
