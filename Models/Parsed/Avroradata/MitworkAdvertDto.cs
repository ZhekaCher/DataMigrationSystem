using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mitwork_advert")]
    public class MitworkAdvertDto
    {
        
        [Column("id")] public long Id { get; set; }
        [Column("advert_id")] public string AdvertId { get; set; }
        [Column("advert_name_kz")] public string AdvertNameKz { get; set; }
        [Column("advert_name_ru")] public string AdvertNameRu { get; set; }
        [Column("advert_start_date")] public DateTime? StartDate { get; set; }
        [Column("advert_finish_date")] public DateTime? FinishDate { get; set; }
        [Column("customer_name")] public string CustomerName { get; set; }
        [Column("customer_bin")] public long? CustomerBin { get; set; }
        [Column("advert_status")] public string AdvertStatus { get; set; }
        [Column("lot_count")] public int LotCount { get; set; }
        [Column("advert_sum_no_nds")] public double? AdvertSumNoNds { get; set; }
        [Column("source_link")] public string SourceLink { get; set; }
        [Column("relevance_time")] public DateTime RelevanceTime { get; set; }
        [Column("advert_method")]public string AdvertMethod { get; set; }
            
        public List<MitworkLotDto> Lots { get; set; }
        public List<MitworkFileDto> AdvertDocumentations { get; set; }
        
    }
}