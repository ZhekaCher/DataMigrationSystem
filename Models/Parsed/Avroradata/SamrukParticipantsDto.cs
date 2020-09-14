using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("samruk_all_participants")]
    public class SamrukParticipantsDto
    {
	    [Key]
        [Column("id")] public long? Id{get; set;}
		[Column("code_bin")] public long? CodeBin{get; set;}
		[Column("name_ru")] public string NameRu{get; set;}
		[Column("name_kk")] public string NameKk{get; set;}
		[Column("legal_form_ru")] public string LegalFormRu{get; set;}
		[Column("legal_form_kk")] public string LegalFormKk{get; set;}
		[Column("field_kk")] public string FieldKk{get; set;}
		[Column("field_ru")] public string FieldRu{get; set;}
		[Column("phone")] public string Phone{get; set;}
		[Column("fax")] public string Fax{get; set;}
		[Column("email")] public string Email{get; set;}
		[Column("site")] public string Site{get; set;}
		[Column("doc_number")] public string DocNumber{get; set;}
		[Column("doc_notify_number")] public string DocNotifyNumber{get; set;}
		[Column("begin_date")] public DateTime? BeginDate{get; set;}
		[Column("end_date")] public DateTime? EndDate{get; set;}
		[Column("issuing_authority")] public string IssuingAuthority{get; set;}
		[Column("issue_date")] public DateTime? IssueDate{get; set;}
		[Column("reissue_date")] public DateTime? ReissueDate{get; set;}
		[Column("code_country")] public string CodeCountry{get; set;}
		[Column("code_kato")] public long? CodeKato{get; set;}
		[Column("legal_address")] public string LegalAddress{get; set;}
		[Column("actual_address")] public string ActualAddress{get; set;}
		[Column("director_fullname")] public string DirectorFullname{get; set;}
		[Column("director_iin")] public long? DirectorIin{get; set;}
		[Column("position_ru")] public string PositionRu{get; set;}
		[Column("position_kk")] public string PositionKk{get; set;}
		[Column("customer")] public bool? Customer{get; set;}
		[Column("supplier")] public bool? Supplier{get; set;}
		[Column("observer")] public bool? Observer{get; set;}
		[Column("authorized_body")] public bool? AuthorizedBody{get; set;}
		[Column("second_tier_bank")] public bool? SecondTierBank{get; set;}
		[Column("auditor_organization")] public bool? AuditorOrganization{get; set;}
		[Column("audit_tyoe")] public string AuditTyoe{get; set;}
		[Column("genuine_supplier")] public bool? GenuineSupplier{get; set;}
		[Column("bad_supplier")] public bool? BadSupplier{get; set;}
		[Column("invalid_company")] public bool? InvalidCompany{get; set;}
		[Column("commodity_producer")] public bool? CommodityProducer{get; set;}
		[Column("terror_finance_company")] public bool? TerrorFinanceCompany{get; set;}
		[Column("subsoil_user")] public bool? SubsoilUser{get; set;}
		[Column("bad_participant")] public bool? BadParticipant{get; set;}
		[Column("resident")] public bool? Resident{get; set;}
		[Column("has_parent")] public bool? HasParent{get; set;}
		[Column("parent_company_bin")] public long? ParentCompanyBin{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
    }
}