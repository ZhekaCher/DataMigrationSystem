using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_producer_company")]
    public class ProducersSk
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("bin")]
        public  long Bin { get; set; }
        [Column("producer_type")]
        public int ProducerType { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}