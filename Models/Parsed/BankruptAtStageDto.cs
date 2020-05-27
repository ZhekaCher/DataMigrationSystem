using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    
    [Table("bankrupt_at_stage")]
    public class BankruptAtStageDto
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("region")] public string Region{get; set;}
        [Column("name")] public string Name{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("rnn")] public string Rnn{get; set;}
        [Column("address")] public string Address{get; set;}
        [Column("director_fullname")] public string DirectorFullname{get; set;}
        [Column("director_iin")] public string DirectorIin{get; set;}
        [Column("founders_fullname")] public string FoundersFullname{get; set;}
        [Column("founders_iin")] public string FoundersIin{get; set;}
        [Column("type_of_service")] public string TypeOfService{get; set;}
        [Column("date_of_court_decision")] public DateTime? DateOfCourtDecision{get; set;}
        [Column("date_of_entry_into_force")] public DateTime? DateOfEntryIntoForce{get; set;}
        [Column("admin_fullname")] public string AdminFullname{get; set;}
        [Column("admin_iin")] public string AdminIin{get; set;}
        [Column("admin_email")] public string AdminEmail{get; set;}
        [Column("admin_phone")] public string AdminPhone{get; set;}
        [Column("admin_address")] public string AdminAddress{get; set;}
        [Column("date_of_relevance")] public DateTime? DateOfRelevance{get; set;}
		
    }
}