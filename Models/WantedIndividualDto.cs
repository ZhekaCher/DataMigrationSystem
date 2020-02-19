using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("wanted_individuals")]
    public class WantedIndividualDto
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
        public string DocumentType { get; set; }
        [Column("code_of_document")]
        public string CodeOfDocument { get; set; }
        [Column("issued_by")]
        public string IssuedBy { get; set; }
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
        public string  Race{ get; set; }
        [Column("list")]
        public string  List{ get; set; }
        [Column("searching_reason")]
        public string  SearchingReason{ get; set; }
        [Column("nationality")]
        public string  Nationality{ get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }

    }
}