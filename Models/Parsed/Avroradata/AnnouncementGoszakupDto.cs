using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("announcement_goszakup")]
	public class AnnouncementGoszakupDto
	{
		[Key] [Column("id")] public long Id{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("number_anno")] public string NumberAnno{get; set;}
		[Column("organizator_biin")] public long? OrganizatorBiin{get; set;}
		[Column("count_lots")] public int? CountLots{get; set;}
		[Column("total_sum")] public double? TotalSum{get; set;}
		[Column("start_date")] public DateTime? StartDate{get; set;}
		[Column("end_date")] public DateTime? EndDate{get; set;}
		[Column("publish_date")] public DateTime? PublishDate{get; set;}
		[Column("type_trade")] public string TypeTrade{get; set;}
		[Column("trade_method")] public string TradeMethod{get; set;}
		[Column("subject_type")] public string SubjectType{get; set;}
		[Column("buy_status")] public string BuyStatus{get; set;}
		public List<LotGoszakupDto> Lots { get; set; }
		public List<AnnouncementFileGoszakupDto> Files { get; set; }
	}
}
