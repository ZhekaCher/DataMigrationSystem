using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("company")]
    public class CompanyDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("region")]
        public int Region { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("name_kz")]
        public string NameKz { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("registration_date")]
        public DateTime? RegistrationDate { get; set; }
        [Column("oked_code")]
        public string OkedCode { get; set; }
        [Column("activity_name_ru")]
        public string ActivityNameRu { get; set; }
        [Column("activity_name_kz")]
        public string ActivityNameKz { get; set; }
        [Column("second_oked_code")]
        public string SecondOkedCode { get; set; }
        [Column("krp_code")]
        public int KrpCode { get; set; }
        [Column("krp_name_ru")]
        public string KrpNameRu { get; set; }
        [Column("krp_name_kz")]
        public string KrpNameKz { get; set; }
        [Column("kato_code")]
        public int KatoCode { get; set; }
        [Column("settlement_name_ru")]
        public string SettlementNameRu { get; set; }
        [Column("settlement_name_kz")]
        public string SettlementNameKz { get; set; }
        [Column("legal_address")]
        public string LegalAddress { get; set; }
        [Column("name_head")]
        public string NameHead { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
        
    }
}