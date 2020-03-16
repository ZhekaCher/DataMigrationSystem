using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{ 
    [Table("enforcement_debt_type")]
    public class EnforcementDebtType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set;}
        [Column("name")]
        public string Name { get; set; }
        [Column("name_kk")]
        public string NameKk { get; set; }
    }
    [Table("egov_enforcement_debt")]
    public class EgovEnforcementDebt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public string Id { get; set; }
        [Column("type_id")]
        public int? TypeId { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
        [Column("biin")]
        public long IinBin { get; set; }
        [Column("number")]
        public string Number { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
        [Column("agency_kk")]
        public string AgencyKk { get; set; }
        [Column("agency_ru")]
        public string AgencyRu { get; set; }
        [Column("judicial_executor_kk")]
        public string JudicialExecutorKk { get; set; }
        [Column("judicial_executor_ru")]
        public string JudicialExecutorRu { get; set; }
        [Column("enforcement_start_date")]
        public DateTime? EnforcementStartDate { get; set; }
        [Column("restriction_start_date")]
        public DateTime? RestrictionStartDate { get; set; }
    }
}