using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("court_case")]
    public class CourtCaseDto
    {
        [Key]
        [Column("number")]
        public string Number { get; set; }
        [Column("sides")]
        public string Sides { get; set; }
        [Column("court")]
        public string Court { get; set; }
        [Column("result")]
        public string Result { get; set; }
        [Column("category")]
        public string Category { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("document_type")]
        public string DocumentType { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("case_type")]
        public string CaseType { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
    }
    [Table("court_case_entity")]
    public class CourtCaseEntityDto
    {
        [Key]
        [Column("number")]
        public string Number{ get; set; }
        [Column("iin_bin")]
        public long IinBin { get; set; }
    }
}