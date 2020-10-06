using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("kazpatents")]
    public class KazpatentDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] public long Id { get; set; }
        [Column("certificate_number")] public string CertificateNumber { get; set; }
        [Column("reg_date")] public DateTime RegDate { get; set; }
        [Column("receipt_date")] public DateTime ReceiptDate { get; set; }
        [Column("patent_type")] public string PatentType { get; set; }
        [Column("full_name")] public string FullName { get; set; }
        [Column("patent_name")] public string PatentName { get; set; }
        [Column("create_date")] public DateTime? CreateDate { get; set; }
        [Column("status")] public string Status { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
        public List<KazPatentOwnerDto> KazPatentOwnerDtos { get; set; }
    }

    [Table("kazpatent_iins")]
    public class KazPatentOwnerDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] public long Id { get; set; }
        [Column("iin")] public long Iin { get; set; }
        [Column("patent_id")] public long PatentId { get; set; }
    }
}