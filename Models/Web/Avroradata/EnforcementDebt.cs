using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("enforcement_debt")]
    public class EnforcementDebt
    {
        [Key]
        [Column("uid")]
        public string Uid { get; set; }
        [Column("number")]
        public string Number { get; set; }
        [Column("essence_requirements")]
        public string EssenceRequirements { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("debtor")]
        public string Debtor { get; set; }
        [Column("claimer")]
        public string Claimer { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
        [Column("judicial_executor")]
        public string JudicialExecutor { get; set; }
        [Column("history")]
        public string History { get; set; }
        [Column("judicial_doc")]
        public string JudicialDoc { get; set; }
        [Column("agency")]
        public string Agency { get; set; }
        [Column("status")] 
        public bool Status { get; set; }
        [Column("type_id")]
        public int? TypeId { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("biin")]
        public long IinBin { get; set; }

    }
    
    [Table("enforcement_debt_type")]
    public class EnforcementDebtType : BaseReferenceEntity
    {

    }
    [Table("enforcement_debt_history")]
    public class EnforcementDebtHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column("biin")]
        public long IinBin { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("count")]
        public int Count { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
    }

}