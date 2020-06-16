using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
	[Table("samruk_lots")]
	public class SamrukLotsDto
    {
        [Column("id")] public long? Id{get; set;}
		[Column("advert_id")] public long? AdvertId{get; set;}
		[Column("lot_id")] public long? LotId{get; set;}
		[Column("name_russian")] public string NameRussian{get; set;}
		[Column("name_kazakh")] public string NameKazakh{get; set;}
		[Column("name_english")] public string NameEnglish{get; set;}
		[Column("tru_code")] public string TruCode{get; set;}
		[Column("tru_detail_russian")] public string TruDetailRussian{get; set;}
		[Column("tru_detail_kazakh")] public string TruDetailKazakh{get; set;}
		[Column("tru_detail_english")] public string TruDetailEnglish{get; set;}
		[Column("acceptance_begin_datetime")] public DateTime? AcceptanceBeginDatetime{get; set;}
		[Column("acceptance_end_datetime")] public DateTime? AcceptanceEndDatetime{get; set;}
		[Column("customer_bin")] public string CustomerBin{get; set;}
		[Column("customer_russian")] public string CustomerRussian{get; set;}
		[Column("organizer_bin")] public string OrganizerBin{get; set;}
		[Column("organizer_russian")] public string OrganizerRussian{get; set;}
		[Column("tender_type")] public string TenderType{get; set;}
		[Column("tender_subject_type")] public string TenderSubjectType{get; set;}
		[Column("kato_code")] public string KatoCode{get; set;}
		[Column("kato_russian")] public string KatoRussian{get; set;}
		[Column("tender_location")] public string TenderLocation{get; set;}
		[Column("delivery_country_code")] public string DeliveryCountryCode{get; set;}
		[Column("delivery_country_russian")] public string DeliveryCountryRussian{get; set;}
		[Column("mkei_code")] public string MkeiCode{get; set;}
		[Column("mkei_russian")] public string MkeiRussian{get; set;}
		[Column("delivery_kato")] public string DeliveryKato{get; set;}
		[Column("delivery_kato_russian")] public string DeliveryKatoRussian{get; set;}
		[Column("delivery_location")] public string DeliveryLocation{get; set;}
		[Column("duration_month")] public string DurationMonth{get; set;}
		[Column("amount")] public string Amount{get; set;}
		[Column("price")] public double? Price{get; set;}
		[Column("sum_tru_no_nds")] public double? SumTruNoNds{get; set;}
		[Column("starting_price")] public double? StartingPrice{get; set;}
		[Column("tender_priority")] public string TenderPriority{get; set;}
		[Column("add_attribute_russian")] public string AddAttributeRussian{get; set;}
		[Column("add_attribute_kazakh")] public string AddAttributeKazakh{get; set;}
		[Column("simple_status")] public string SimpleStatus{get; set;}
		[Column("lot_status")] public string LotStatus{get; set;}
		[Column("advert_name_russian")] public string AdvertNameRussian{get; set;}
		[Column("advert_name_kazakh")] public string AdvertNameKazakh{get; set;}
		[Column("advert_name_english")] public string AdvertNameEnglish{get; set;}
		[Column("advert_status")] public string AdvertStatus{get; set;}
		[Column("flag_prequalification")] public bool? FlagPrequalification{get; set;}
		[Column("is_disabled")] public string IsDisabled{get; set;}
		[Column("pko_supplier_status")] public string PkoSupplierStatus{get; set;}
		[Column("pko_level")] public string PkoLevel{get; set;}
		[Column("documents")] public string Documents{get; set;}
		[Column("participation_id")] public string ParticipationId{get; set;}
		[Column("begin_datetime")] public DateTime? BeginDatetime{get; set;}
		[Column("end_datetime")] public DateTime? EndDatetime{get; set;}
		[Column("transferred")] public string Transferred{get; set;}
		[Column("additional_price_acceptance_begin_datetime")] public DateTime? AdditionalPriceAcceptanceBeginDatetime{get; set;}
		[Column("additional_price_acceptance_end_datetime")] public DateTime? AdditionalPriceAcceptanceEndDatetime{get; set;}
		[Column("second_additional_price_acceptance_begin_datetime")] public DateTime? SecondAdditionalPriceAcceptanceBeginDatetime{get; set;}
		[Column("second_additional_price_acceptance_end_datetime")] public DateTime? SecondAdditionalPriceAcceptanceEndDatetime{get; set;}
		[Column("additional_price_acceptance_stage")] public string AdditionalPriceAcceptanceStage{get; set;}
		[Column("can_submit_commercial_offer")] public string CanSubmitCommercialOffer{get; set;}
		[Column("count")] public string Count{get; set;}
    }
}