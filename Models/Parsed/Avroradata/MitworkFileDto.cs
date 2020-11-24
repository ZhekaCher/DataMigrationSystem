using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mitwork_files")]
    public class MitworkFileDto
    {
        [Column("id")]
        public long Id { get; set; }
            
        [Column("advert_id")] public string AdvertId { get; set; }
        [Column("lot_id")] public string LotId { get; set; }
        [Column("name")] public string DocName { get; set; }
        [Column("category")] public string DocCategory { get; set; }
        [Column("file_path")] public string DocFilePath { get; set; }
        [Column("source_link")] public string DocSourceLink { get; set; }
        [Column("file_uid")] public string DocFileUid { get; set; }
        [Column("file_iid")]public string Iid { get; set; }
        [Column("relevance_time")] public DateTime RelevanceTime { get; set; }
    }
}