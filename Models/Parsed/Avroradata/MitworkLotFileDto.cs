using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mitwork_lot_documentation")]
    public class MitworkLotFileDto
    {
        [Column("id")]
        public long Id { get; set; }
            
        [Column("lot_id")] public string LotId { get; set; }
        [Column("name")] public string LotDocName { get; set; }
        [Column("category")] public string LotDocCategory { get; set; }
        [Column("file_path")] public string LotDocFilePath { get; set; }
        [Column("source_link")] public string LotDocSourceLink { get; set; }
        [Column("file_uid")] public string LotDocFileUid { get; set; }
        [Column("relevance_time")] public DateTime RelevanceTime { get; set; }
    }
}