using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("bankrupt_at_stage")]
    public class BankruptAtStage
    {
        [Column("id_region")] public long? IdRegion{get; set;}
        [Column("id_type_of_service")] public long? IdTypeOfService{get; set;}
        [Column("id_address")] public long? IdAddress{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("date_of_entry_into_force")] public DateTime? DateOfEntryIntoForce{get; set;}
        [Column("date_of_court_decision")] public DateTime? DateOfCourtDecision{get; set;}
        [Key] [Column("biin_companies")] public long? BiinCompanies{get; set;}
    }
    
    [Table("bankrupt_address")]
    public class BankruptSAddress
    {
        [Key] [Column("i")] public long? I{get; set;}
        [Column("name")] public string Name{get; set;}
    }
    
    [Table("region")]
    public class RegionS
    {
        [Key] [Column("i")] public long? I{get; set;}
        [Column("name")] public string Name{get; set;}
    }
    
    [Table("type_of_service")]
    public class TypeOfServiceS
    {
        [Key] [Column("i")] public long? I{get; set;}
        [Column("name")] public string Name{get; set;}
    }
}