using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("bankrupt_completed")]
    public class BankruptCompletedDto
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("region")] public string Region{get; set;}
        [Column("name")] public string Name{get; set;}
        [Column("bin")] public string Bin{get; set;}
        [Column("rnn")] public string Rnn{get; set;}
        [Column("address")] public string Address{get; set;}
        [Column("director_fullname")] public string DirectorFullname{get; set;}
        [Column("director_iin")] public string DirectorIin{get; set;}
        [Column("founders_fullname")] public string FoundersFullname{get; set;}
        [Column("founders_iin")] public string FoundersIin{get; set;}
        [Column("type_of_service")] public string TypeOfService{get; set;}
        [Column("date_decision")] public DateTime? DateDecision{get; set;}
        [Column("date_entry")] public DateTime? DateEntry{get; set;}
        [Column("date_decision_end")] public DateTime? DateDecisionEnd{get; set;}
        [Column("date_entry_end")] public DateTime? DateEntryEnd{get; set;}
        [Column("date_of_relevance")] public DateTime? DateOfRelevance{get; set;}
		
    }
}