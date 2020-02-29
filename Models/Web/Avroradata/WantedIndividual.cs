using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("wanted_individuals")]
    public class WantedIndividual
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("iin")]
        public long Iin { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Column("gender")] 
        public int Gender { get; set; }
        [Column("id_document_type")]
        public int? DocumentType { get; set; }
        [Column("code_of_document")]
        public string CodeOfDocument { get; set; }
        [Column("id_issued_by")]
        public int? IssuedBy { get; set; }
        [Column("issue_date")]
        public DateTime?  IssueDate{ get; set; }
        [Column("searching_authority")]
        public string  SearchingAuthority{ get; set; }
        [Column("authority_phone")]
        public string  AuthotityPhone{ get; set; }
        [Column("reception_phone")]
        public string  ReceptionPhone{ get; set; }
        [Column("date_of_birth")]
        public DateTime?  Birthday{ get; set; }
        [Column("id_race_type")]
        public int?  Race{ get; set; }
        [Column("id_list_type")]
        public int?  ListId{ get; set; }
        [Column("searching_reason")]
        public string  SearchingReason{ get; set; }
        [Column("id_nationality")]
        public int?  Nationality{ get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }  = DateTime.Now;
    }

    [Table("nationality")]
    public class Nationality : BaseReferenceEntity
    {
        
    }
    [Table("issued")]
    public class Issued : BaseReferenceEntity
    {
        
    }
    [Table("wanted_individual_types")]
    public class ListType : BaseReferenceEntity
    {
        
    }
    [Table("race_type")]
    public class RaceType : BaseReferenceEntity
    {
        
    }
    [Table("document")]
    public class Document : BaseReferenceEntity
    {
        
    }
    
}