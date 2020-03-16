using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("nds_detail")]
    public class NdsDetails
    {
        [Column("id")] public long? Id{get; set;}
        [Column("rnn")] public long? Rnn{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("start_date")] public DateTime? StartDate{get; set;}
        [Column("finish_date")] public DateTime? FinishDate{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("reason_id")] public int? ReasonId{get; set;}
    }

    [Table("nds_detail_reason")]
    public class NdsDetailReason
    {
        [Column("id")] public int? Id{get; set;}
        [Column("name")] public string? Name{get; set;}
    }
}