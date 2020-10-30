using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    public class ErgTenderDto
    {
        [Table("erg_tender")]
        public class ErgTender
        {
            [Key][Column("id")] public long Id { get; set; }
            [Column("full_name")] public string FullName { get; set; }
            [Column("main_category")] public string MainCategory { get; set; }
            [Column("num")] public string Num { get; set; }
            [Column("publish_date")] public DateTime? PublishDate { get; set; }
            [Column("end_date")] public DateTime? EndDate { get; set; }
            [Column("customer")] public string Customer { get; set; }
            [Column("bin")] public long Bin { get; set; }
            [Column("type_name")] public string TypeName { get; set; }
            [Column("purchase")] public string Purchase { get; set; }
            [Column("status")] public string Status { get; set; }
            [Column("concourse_id")] public long ConcourseId { get; set; }
            public List<ErgTenderPositions> ErgTenderPositionses { get; set; } 
        }

        [Table("erg_tender_positions")]
        public class ErgTenderPositions
        {
            [Key][Column("id")] public long Id { get; set; }
            [Column("concourse_id")] public long ConcourseId { get; set; }
            [Column("customer_name")] public string CustomerName { get; set; }
            [Column("item_name")] public string ItemName { get; set; }
            [Column("delivery_date")] public DateTime? DeliveryDate { get; set; }
            [Column("unit")] public string Unit { get; set; }
            [Column("count")] public double Count { get; set; }
            [Column("tru_code")] public string TruCode { get; set; }
        }
    }
}