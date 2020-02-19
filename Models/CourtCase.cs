using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("court_case")]
    public class CourtCase
    {
        [Key]
        [Column("number")]
        public string Number { get; set; }
        [Column("sides")]
        public string Sides { get; set; }
        [Column("court_id")]
        public int? CourtId { get; set; }
        [Column("result")]
        public string Result { get; set; }
        [Column("category_id")]
        public int? CategoryId { get; set; }
        [Column("date")]
        public DateTime? Date { get; set; }
        [Column("document_type_id")]
        public int? DocumentTypeId { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("case_type_id")]
        public int? CaseTypeId { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
    }
    [Table("court")]
    public class Court : BaseReferenceEntity
    {
    }
    [Table("court_case_document_type")]
    public class CourtCaseDocumentType : BaseReferenceEntity
    {
    }
    [Table("court_case_type")]
    public class CourtCaseType : BaseReferenceEntity
    {
    }
    [Table("court_case_category")]
    public class CourtCaseCategory : BaseReferenceEntity
    {
    }
    public class CourtCaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("number")]
        public string Number { get; set; }
    }
    [Table("i_court_case_entity")]
    public class IndividualCourtCaseEntity : CourtCaseEntity
    {
        [Column("iin")]
        public long IinBin { get; set; }
    }
    [Table("court_case_entity")]
    public class CompanyCourtCaseEntity : CourtCaseEntity
    {
        [Column("bin")]
        public long IinBin { get; set; }
    }
}