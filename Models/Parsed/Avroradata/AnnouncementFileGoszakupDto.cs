using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
	[Table("announcement_file_goszakup")]
	public class AnnouncementFileGoszakupDto
	{
		[Key] [Column("id")] public long Id{get; set;}
		[Column("anno_id")] public long? AnnoId{get; set;}
		[Column("original_name")] public string OriginalName{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("link")] public string Link{get; set;}
		// public AnnouncementGoszakupDto Announcement { get; set; }
	}
}
