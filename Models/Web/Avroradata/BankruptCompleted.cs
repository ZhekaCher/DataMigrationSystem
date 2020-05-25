using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("bankrupt_completed")]
    public class BankruptCompleted
    {
        [Column("id_region")] public long? IdRegion{get; set;}
        [Column("id_address")] public long? IdAddress{get; set;}
        [Column("id_type_of_service")] public long? IdTypeOfService{get; set;}
        [Column("date_decision")] public DateTime? DateDecision{get; set;}
        [Column("date_entry")] public DateTime? DateEntry{get; set;}
        [Column("date_decision_end")] public DateTime? DateDecisionEnd{get; set;}
        [Column("date_entry_end")] public DateTime? DateEntryEnd{get; set;}
        [Column("date_of_relevance")] public DateTime? RelevanceDate{get; set;}
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key] [Column("biin_companies")] public long? BiinCompanies{get; set;}
    }
}