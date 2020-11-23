namespace DataMigrationSystem.Models.Parsed.Avroradata.CompaniesFl
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CompaniesFlDto
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("region")]
        public int Region { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("registration_date")]
        public DateTime? RegistrationDate { get; set; }
        [Column("oked_code")]
        public string OkedCode { get; set; }
        [Column("activity_name_ru")]
        public string ActivityNameRu { get; set; }
        [Column("second_oked_code")]
        public string SecondOkedCode { get; set; }
        [Column("krp_code")]
        public int KrpCode { get; set; }
        [Column("krp_name_ru")]
        public string KrpNameRu { get; set; }
        [Column("kato_code")]
        public int KatoCode { get; set; }
        [Column("kato_address")]
        public string KatoAddress { get; set; }
        [Column("name_head")]
        public string NameHead { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
        [Column("ip")]
        public bool Ip { get; set; }
    }
}