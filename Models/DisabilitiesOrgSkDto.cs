using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("disabilities_organizations_sk")]
    public class DisabilitiesOrgSkDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("legal_form")]
        public long LegalForm { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("bin")]
        public  long Bin { get; set; }
        [Column("ceo")]
        public string Ceo { get; set; }
        [Column("contacts")]
        public string Contacts { get; set; }
        [Column("producer_type")]
        public string ProducerType { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}