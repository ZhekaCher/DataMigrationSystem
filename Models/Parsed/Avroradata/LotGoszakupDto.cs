using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("lot_goszakup")]
	public class LotGoszakupDto
	{
		[Key] [Column("id")] public long Id{get; set;}
		[Column("anno_id")] public long? AnnoId{get; set;}
		[Column("lot_number")] public string LotNumber{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("description_ru")] public string DescriptionRu{get; set;}
		[Column("customer_bin")] public long? CustomerBin{get; set;}
		[Column("amount")] public double? Amount{get; set;}
		[Column("count")] public double? Count{get; set;}
		[Column("lot_status")] public string LotStatus{get; set;}
		[Column("tru")] public string Tru{get; set;}
		[Column("supply_date_ru")] public string SupplyDateRu{get; set;}
		[Column("units")] public string Units{get; set;}
		[Column("delivery_place")] public string DeliveryPlace{get; set;}
		// public AnnouncementGoszakupDto Announcement { get; set; }
		public List<LotFileGoszakupDto> Files { get; set; }
	}
}
