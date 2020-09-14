using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("samruk_files")]
    public class SamrukFilesDto
    {
        [Column("id")] public long? Id{get; set;}
        [Column("advert_id")] public long? AdvertId{get; set;}
        [Column("lot_id")] public long? LotId{get; set;}
        [Column("category")] public string Category{get; set;}
        [Column("uid")] public string Uid{get; set;}
        [Column("name")] public string Name{get; set;}
        [Column("content_type")] public string ContentType{get; set;}
        [Column("file_path")] public string FilePath{get; set;}
    }
}