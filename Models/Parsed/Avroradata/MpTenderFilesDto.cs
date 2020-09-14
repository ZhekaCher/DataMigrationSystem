using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mp_lot_file")]
    public class MpTenderFilesDto
    {
      
        [Column("id")] public long? Id{get; set;}
        [Column("advert_id")] public long? AdvertId{get; set;}
        [Column("lot_id")] public long? LotId{get; set;}
        [Column("name")] public string Name {get; set;}
        [Column("file_path")] public string FilePath{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("local_file_path")] public string LocalFilePath { get; set; }
    }
}