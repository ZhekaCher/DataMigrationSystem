using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("local_certificate")]
    public class LocalCertificatesDto
    {
        [Key] 
        [Column("id")]
        public long Id { get; set; }
        [Column("code_rpp")]
        public long CodeRpp { get; set; }
        [Column("rpp_name")]
        public string RppName { get; set; }
        [Column("cert_num")]
        public string CertNum { get; set; }
        [Column("blank_num")]
        public string BlankNum { get; set; }
        [Column("year")]
        public string Year { get; set; }
        [Column("certificate_purpose")]
        public string CertificatePurpose { get; set; }
        [Column("manufacturer_iin_biin")]
        public long ManufacturerIinBiin { get; set; }
        [Column("manufacturer_name")]
        public string ManufacturerName { get; set; }
        [Column("manufacturer_address")]
        public string ManufacturerAddress { get; set; }
        [Column("goods_name")]
        public string GoodsName { get; set; }
        [Column("code_tn")]
        public string CodeTn { get; set; }
        [Column("code_kp")]
        public string CodeKp {get; set; }
        [Column("goods_count")]
        public string GoodsCount { get; set; }
        [Column("unit_metric")]
        public string? UnitMetric { get; set; }
        [Column("code_of_unit_metric")]
        public string? CodeOfUnitMetric { get; set; }
        [Column("origin_criterion")]
        public string OriginCriterion { get; set; }
        [Column("percentage")]
        public string Percentage { get; set; }
        [Column("receiver_iin_biin")]
        public long? ReceiverIinBiin { get; set; }
        [Column("receiver_name")]
        public string? ReceiverName { get; set; }
        [Column("receiver_address")]
        public string ReceiverAddress { get; set; }
        [Column("certificate_end_date")]
        public DateTime CertificateEndDate { get; set; }
        [Column("certificate_status")]
        public string CertificateStatus { get; set; }
        [Column("issue_date")]
        public DateTime IssueDate { get; set; }
        [Column("category")]
        public string Category { get; set; }
        
    }
    [Table("industrial_certificate")]
    public class IndustrialCertificatesDto
    {
        [Key] 
        [Column("id")]
        public long Id { get; set; }
        [Column("iin_biin")]
        public long IinBiin { get; set; }
        [Column("registration_number")]
        public string RegistrationNumber { get; set; }
        [Column("manufacturer_name")]
        public string ManufacturerName { get; set; }
        [Column("activity")]
        public string Activity { get; set; }
        [Column("region_by_kato")]
        public string RegionByKato { get; set; }
        [Column("legal_address")]
        public string LegalAddress { get; set; }
        [Column("street_address")]
        public string StreetAddress { get; set; }
        [Column("fact_address")]
        public string FactAddress { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("email_address")]
        public string EmailAddress { get; set; }
        [Column("website")]
        public string Website { get; set; }
        [Column("goods_name")]
        public string GoodsName { get; set; }
        [Column("goods_count_per_year")]
        public string GoodsCountPerYear {get; set; }
        [Column("tn")]
        public string? Tn { get; set; }
        [Column("kp")]
        public string Kp { get; set; }
        [Column("assesment_doc")]
        public string? AssesmentDoc { get; set; }
        [Column("issue_date")]
        public DateTime? IssueDate { get; set; }
        [Column("end_date")]
        public DateTime? EndDate { get; set; }
        [Column("licence_num")]
        public string? LicenceNum { get; set; }
        [Column("workers_count")]
        public string WorkersCount { get; set; }
        [Column("reester_insert_date")]
        public DateTime? ReesterInsertDate { get; set; }
        [Column("changes_date")]
        public DateTime? ChangesDate { get; set; }
        [Column("actualization_date")]
        public DateTime? ActualizationDate { get; set; }
    }
    [Table("export_certificate")]
    public class ExportCertificatesDto
    {
        [Key] 
        [Column("id")]
        public long Id { get; set; }
        [Column("cert_num")]
        public string CertNum { get; set; }
        [Column("rpp_name")]
        public string RppName { get; set; }
        [Column("cert_form")]
        public string CertForm { get; set; }
        [Column("blank_num")]
        public string BlankNum { get; set; }
        [Column("manufacturer_iin_biin")]
        public long ManufacturerIinBiin { get; set; }
        [Column("manufacturer_name")]
        public string ManufacturerName { get; set; }
        [Column("manufacturer_address")]
        public string ManufacturerAddress { get; set; }
        [Column("goods_name")]
        public string GoodsName { get; set; }
        [Column("code_tn")]
        public string CodeTn { get; set; }
        [Column("origin_criterion")]
        public string OriginCriterion { get; set; }
        [Column("goods_country")]
        public string GoodsCountry { get; set; }
        [Column("receiver_country")]
        public string ReceiverCountry { get; set; }
        [Column("certificate_status")]
        public string CertificateStatus {get; set; }
        [Column("issue_date")]
        public DateTime? IssueDate { get; set; }
    }
    
}