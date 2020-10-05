using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("contract_unit_goszakup")]
	public class ContractUnitGoszakupDto
	{
		[Key] [Column("id")] public long? Id{get; set;}
		[Column("source_unique_id")] public long? SourceUniqueId{get; set;}
		[Column("contract_id")] public long? ContractId{get; set;}
		[Column("item_price")] public double? ItemPrice{get; set;}
		[Column("item_price_wnds")] public double? ItemPriceWnds{get; set;}
		[Column("quantity")] public double? Quantity{get; set;}
		[Column("total_sum")] public double? TotalSum{get; set;}
		[Column("total_sum_wnds")] public double? TotalSumWnds{get; set;}
		public PlanGoszakupDto Plan { get; set; }
	}
}
