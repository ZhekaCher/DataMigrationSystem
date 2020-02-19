using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("leaving_restriction")]
    public class LeavingRestrictionDto
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
        [Column("iin_bin")]
        public long IinBin { get; set; }

    }
}