using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("leaving_restriction")]
    public class LeavingRestriction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("debtor")]
        public string Debtor { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("judicial_executor")]
        public string JudicialExecutor { get; set; }
        [Column("cause")]
        public string Cause { get; set; }
        [Column("biin")]
        public long IinBin { get; set; }
    }

}