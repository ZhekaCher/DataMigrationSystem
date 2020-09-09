using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("new_bankrupt_at_stage")]
    public class NewBankruptAtStage
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("date_of_court_decision")] public DateTime? DateOfCourtDecision{get; set;}
        [Column("date_of_entry_into_force")] public DateTime? DateOfEntryIntoForce{get; set;}
        [Column("date_of_relevance")] public DateTime? DateOfRelevance{get; set;}
    }
}