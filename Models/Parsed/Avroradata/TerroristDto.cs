using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("terrorists")]
    public class TerroristDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("middle_name")]
        public string MiddleName { get; set; }
        [Column("birthday")]
        public DateTime? Birthday { get; set; }
        [Column("note")]
        public string Note { get; set; }
        [Column("correction")]
        public string Correction { get; set; }
        [Column("iin")]
        public long Iin { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate{ get; set; }
        [Column("status")]
        public int Status{ get; set; }
    }
}