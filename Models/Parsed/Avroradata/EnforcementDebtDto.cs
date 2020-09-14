using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("enforcement_debt")]
    public class EnforcementDebtDto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
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
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
        [Column("type_kk")]
        public string TypeKk { get; set; }
        [Column("type_ru")]
        public string TypeRu { get; set; }
    }
}