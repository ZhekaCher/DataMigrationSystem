using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("mp_advert")]
    public class MpTenderDto
    {

        [Column("id")] public long Id { get; set; }
        [Column("advert_id")] public long AdvertId { get; set; }
        [Column("advert_name")] public string AdvertName { get; set; }

        [Column("start_auc")] public DateTime? StartAuc { get; set; }
        [Column("end_auc")] public DateTime? EndAuc { get; set; }
        [Column("initiator_of_auc")] public string InitiatorOfAuc { get; set; }
        [Column("bin")] public long Bin { get; set; }
        [Column("company_addres")] public string CompanyAddres { get; set; }
        [Column("status_of_auc")] public string StatusOfAuc { get; set; }
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("type_of_auction")] public string TypeOfAuction { get; set; }
        [Column("count")] public int Count  { get; set; }
        [Column("sum_of_lots")] public double? SumOfLots  { get; set; }
        
        public List<MpTenderLotsDto> Lots { get; set; }

    }

}  