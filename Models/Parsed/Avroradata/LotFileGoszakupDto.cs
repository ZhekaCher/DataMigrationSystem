using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("lot_file_goszakup")]
	public class LotFileGoszakupDto
	{
		[Key][Column("id")] public long Id{get; set;}
		[Column("lot_id")] public long? LotId{get; set;}
		[Column("original_name")] public string OriginalName{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("link")] public string Link{get; set;}
		// public LotGoszakupDto Lot { get; set; }
	}
}
