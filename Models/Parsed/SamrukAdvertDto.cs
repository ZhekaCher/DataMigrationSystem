using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
	[Table("samruk_advert")]
	public class SamrukAdvertDto
    {
        [Column("id")] public long? Id{get; set;}
		[Column("advert_id")] public long? AdvertId{get; set;}
		[Column("name_russian")] public string NameRussian{get; set;}
		[Column("name_kazakh")] public string NameKazakh{get; set;}
		[Column("name_english")] public string NameEnglish{get; set;}
		[Column("acceptance_begin_datetime")] public DateTime? AcceptanceBeginDatetime{get; set;}
		[Column("acceptance_end_datetime")] public DateTime? AcceptanceEndDatetime{get; set;}
		[Column("acceptance_supplement_begin_datetime")] public DateTime? AcceptanceSupplementBeginDatetime{get; set;}
		[Column("acceptance_supplement_end_datetime")] public DateTime? AcceptanceSupplementEndDatetime{get; set;}
		[Column("additional_price_acceptance_begin_datetime")] public DateTime? AdditionalPriceAcceptanceBeginDatetime{get; set;}
		[Column("additional_price_acceptance_end_datetime")] public DateTime? AdditionalPriceAcceptanceEndDatetime{get; set;}
		[Column("second_additional_price_acceptance_begin_datetime")] public DateTime? SecondAdditionalPriceAcceptanceBeginDatetime{get; set;}
		[Column("second_additional_price_acceptance_end_datetime")] public DateTime? SecondAdditionalPriceAcceptanceEndDatetime{get; set;}
		[Column("additional_price_acceptance_stage")] public string AdditionalPriceAcceptanceStage{get; set;}
		[Column("customer_bin")] public string CustomerBin{get; set;}
		[Column("customer_russian")] public string CustomerRussian{get; set;}
		[Column("customers")] public string Customers{get; set;}
		[Column("organizer_bin")] public string OrganizerBin{get; set;}
		[Column("organizer_russian")] public string OrganizerRussian{get; set;}
		[Column("phone")] public string Phone{get; set;}
		[Column("email")] public string Email{get; set;}
		[Column("tender_priority")] public string TenderPriority{get; set;}
		[Column("tender_type")] public string TenderType{get; set;}
		[Column("stage")] public string Stage{get; set;}
		[Column("kato")] public string Kato{get; set;}
		[Column("tender_location")] public string TenderLocation{get; set;}
		[Column("sum_tru_no_nds")] public double? SumTruNoNds{get; set;}
		[Column("flag_complex_tender")] public bool? FlagComplexTender{get; set;}
		[Column("flag_prequalification")] public bool FlagPrequalification{get; set;}
		[Column("flag_consulting")] public bool? FlagConsulting{get; set;}
		[Column("flag_transferred")] public bool? FlagTransferred{get; set;}
		[Column("flag_centralized_tender")] public bool? FlagCentralizedTender{get; set;}
		[Column("simple_status")] public string SimpleStatus{get; set;}
		[Column("advert_status")] public string AdvertStatus{get; set;}
		[Column("participation_id")] public string ParticipationId{get; set;}
		[Column("time_history_begin_date")] public DateTime? TimeHistoryBeginDate{get; set;}
		[Column("time_history_end_date")] public DateTime? TimeHistoryEndDate{get; set;}
		public List<SamrukLotsDto> Lots { get; set; }
		public List<SamrukFilesDto> Documentations { get; set; }
    }
}