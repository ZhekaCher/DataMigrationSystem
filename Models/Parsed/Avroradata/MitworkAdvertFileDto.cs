using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mitwork_advert_documentation")]
    public class MitworkAdvertFileDto
    {
        [Column("id")]
        public long Id { get; set; }
            
        [Column("advert_id")] public string AdvertId { get; set; }
        [Column("name")] public string AdvertDocName { get; set; }
        [Column("category")] public string AdvertDocCategory { get; set; }
        [Column("file_path")] public string AdvertDocFilePath { get; set; }
        [Column("source_link")] public string AdvertDocSourceLink { get; set; }
        [Column("file_uid")] public string AdvertDocFileUid { get; set; }
        [Column("relevance_time")] public DateTime RelevanceTime { get; set; }
    }
}