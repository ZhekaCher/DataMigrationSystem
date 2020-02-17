using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("enforcement_debt")]
    public class EnforcementDebtDto
    {
        [Key]
        [Column("uid")]
        public string Uid { get; set; }
        [Column("essence_requirements")]
        public string EssenceRequirements { get; set; }
        [Column("debtor")]
        public string Debtor { get; set; }
        [Column("agency")]
        public string Agency { get; set; }
        [Column("judicial_executor")]
        public string JudicialExecutor { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("iin_bin")]
        public long IinBin { get; set; }
        public EnforcementDebtDetailDto Detail { get; set; }
    }
    [Table("enforcement_debt_detail")]
    public class EnforcementDebtDetailDto
    {
        [Key]
        [Column("uid")]
        public string Uid { get; set; }
        [Column("number")]
        public string Number { get; set; }
        [Column("uid")]
        public string Claimer { get; set; }
        [Column("uid")]
        public double Amount { get; set; }
        [Column("uid")]
        public string History { get; set; }
        [Column("judicial_doc")]
        public string JudicialDoc { get; set; }
        [Column("type")]
        public string Type { get; set; }
        public EnforcementDebtDto EnforcementDebtDto { get; set; }
    }
}