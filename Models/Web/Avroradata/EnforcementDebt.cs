using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
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
        public bool Status { get; set; } = true;
        [Column("type_id")]
        public int? TypeId { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;

    }
    
    [Table("enforcement_debt_type")]
    public class EnforcementDebtType : BaseReferenceEntity
    {

    }
    [Table("c_enforcement_debt")]
    public class CompanyEnforcementDebt : EnforcementDebt
    {
        [Column("bin")]
        public long IinBin { get; set; }
    }
    
    [Table("i_enforcement_debt")]
    public class IndividualEnforcementDebt : EnforcementDebt{
        [Column("iin")]
        public long IinBin { get; set; }
    }
    
}