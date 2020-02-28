using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_reliable_supplier")]
    public class ReliableSk
    {
        [Column("reason")]
        public string Reason { get; set; }
        [Column("adding_date")]
        public DateTime AddingDate { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("bin")]
        public long? Biin { get; set; }
    }
}