using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("new_bankrupt_completed")]
    public class NewBankruptCompleted
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("date_decision")] public DateTime? DateDecision{get; set;}
        [Column("date_entry")] public DateTime? DateEntry{get; set;}
        [Column("date_decision_end")] public DateTime? DateDecisionEnd{get; set;}
        [Column("date_entry_end")] public DateTime? DateEntryEnd{get; set;}
        [Column("date_of_relevance")] public DateTime? DateOfRelevance{get; set;}
    }
}