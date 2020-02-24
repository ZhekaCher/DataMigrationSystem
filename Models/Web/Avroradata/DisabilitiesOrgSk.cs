using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    public class DisabilitiesOrgSk
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("bin")]
        public  long Bin { get; set; }
        [Column("producer_type")]
        public string ProducerType { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}