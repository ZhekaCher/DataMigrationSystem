using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
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
    }
    
    [Table("c_leaving_restriction")]
    public class CompanyLeavingRestriction : LeavingRestriction
    {
        [Column("bin")]
        public long IinBin { get; set; }
    }
    
    [Table("i_leaving_restriction")]
    public class IndividualLeavingRestriction : LeavingRestriction
    {
        [Column("iin")]
        public long IinBin { get; set; }
    }
}