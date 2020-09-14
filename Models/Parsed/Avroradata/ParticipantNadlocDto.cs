using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("nadloc_participants")]
    public class ParticipantNadlocDto
    {
        [Key] [Column("id")] public int? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("email")] public string Email{get; set;}
        [Column("web_site")] public string WebSite{get; set;}
        [Column("fax")] public string Fax{get; set;}
        [Column("incorporation")] public string Incorporation{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("tel_1")] public string Tel1{get; set;}
        [Column("tel_2")] public string Tel2{get; set;}
        [Column("legal_locality")] public string LegalLocality{get; set;}
        [Column("legal_street")] public string LegalStreet{get; set;}
        [Column("legal_building_num")] public string LegalBuildingNum{get; set;}
        [Column("legal_office_num")] public string LegalOfficeNum{get; set;}
        [Column("actual_locality")] public string ActualLocality{get; set;}
        [Column("actual_street")] public string ActualStreet{get; set;}
        [Column("actual_building")] public string ActualBuilding{get; set;}
        [Column("actual_office_num")] public string ActualOfficeNum{get; set;}
        [Column("chief_name")] public string ChiefName{get; set;}
        [Column("chief_position")] public string ChiefPosition{get; set;}
        [Column("contact_name")] public string ContactName{get; set;}
        [Column("contact_position")] public string ContactPosition{get; set;}
        [Column("contact_tel")] public string ContactTel{get; set;}
        [Column("contact_fax")] public string ContactFax{get; set;}
        [Column("contact_email")] public string ContactEmail{get; set;}
        [Column("reg_date")] public DateTime? RegDate{get; set;}
        [Column("customer_link")] public string CustomerLink{get; set;}
		
    }

    [Table("nadloc_customers")]
    public class CustomerNadlocDto
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
        [Column("status")] public Boolean? Status { get; set; }

    }
    
    [Table("nadloc_suppliers")]
    public class SupplierNadlocDto
    {
        [Key] [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
        [Column("status")] public Boolean? Status { get; set; }

    }
    
}