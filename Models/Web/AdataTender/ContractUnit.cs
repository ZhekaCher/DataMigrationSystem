using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
	[Table("contract_units")]
	public class ContractUnit
	{
		[Key] [Column("id")] public int? Id{get; set;}
		[Column("source_unique_id")] public long? SourceUniqueId{get; set;}
		[Column("source_id")] public long? SourceId{get; set;}
		[Column("item_price")] public double? ItemPrice{get; set;}
		[Column("item_price_wnds")] public double? ItemPriceWnds{get; set;}
		[Column("quantity")] public double? Quantity{get; set;}
		[Column("total_sum")] public double? TotalSum{get; set;}
		[Column("total_sum_wnds")] public double? TotalSumWnds{get; set;}
		[Column("contract_id")] public long? ContractId{get; set;}
		
	}
}
