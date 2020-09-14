using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("wanted_individuals")]
    public class WantedIndividualDto
    {
        [Key]
        [Column("i")]
        public long Id { get; set; }
        [Column("iin")]
        public long Iin { get; set; }
        [Column("surname")]
        public string LastName { get; set; }
        [Column("name")]
        public string FirstName { get; set; }
        [Column("patronym")]
        public string MiddleName { get; set; }
        [Column("genre")] 
        public string Gender { get; set; }
        [Column("document_type")]
        public string DocumentType { get; set; }
        [Column("code_of_document")]
        public string CodeOfDocument { get; set; }
        [Column("issued_by")]
        public string IssuedBy { get; set; }
        [Column("date_of_issue")]
        public DateTime?  IssueDate{ get; set; }
        [Column("searching_authority")]
        public string  SearchingAuthority{ get; set; }
        [Column("authority_phone")]
        public string  AuthotityPhone{ get; set; }
        [Column("reception_phone")]
        public string  ReceptionPhone{ get; set; }
        [Column("date_of_birth")]
        public DateTime?  Birthday{ get; set; }
        [Column("race_type")]
        public string  Race{ get; set; }
        [Column("list_type")]
        public string  List{ get; set; }
        [Column("reason_of_searching")]
        public string  SearchingReason{ get; set; }
        [Column("nationalty")]
        public string  Nationality{ get; set; }
        [Column("date_of_relevance")]
        public DateTime RelevanceDate { get; set; }

    }
}