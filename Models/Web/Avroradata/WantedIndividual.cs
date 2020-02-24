using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("i_lists")]
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
        public string Gender { get; set; }
        [Column("document_type")]
        public int? DocumentType { get; set; }
        [Column("code_of_document")]
        public string CodeOfDocument { get; set; }
        [Column("issued_by")]
        public int? IssuedBy { get; set; }
        [Column("issue_date")]
        public string  IssueDate{ get; set; }
        [Column("searching_authority")]
        public string  SearchingAuthority{ get; set; }
        [Column("authority_phone")]
        public string  AuthotityPhone{ get; set; }
        [Column("reception_phone")]
        public string  ReceptionPhone{ get; set; }
        [Column("birthday")]
        public string  Birthday{ get; set; }
        [Column("race")]
        public int?  Race{ get; set; }
        [Column("list")]
        public int?  ListId{ get; set; }
        [Column("searching_reason")]
        public string  SearchingReason{ get; set; }
        [Column("nationality")]
        public int?  Nationality{ get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }

    [Table("nationality")]
    public class Nationality : BaseReferenceEntity
    {
        
    }
    [Table("issued")]
    public class Issued : BaseReferenceEntity
    {
        
    }
    [Table("i_list_type")]
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