using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("nadloc_tenders")]
    public class AnnouncementNadlocDto
    {
        [Column("tender_id")] public long? TenderId{get; set;}
        [Column("full_id")] public string FullId{get; set;}
        [Column("name")] public string Name{get; set;}
        [Column("customer_name")] public string CustomerName{get; set;}
        [Column("customer_link")] public string CustomerLink{get; set;}
        [Column("price_offers")] public string PriceOffers{get; set;}
        [Column("purchase_method")] public string PurchaseMethod{get; set;}
        [Column("status")] public string Status{get; set;}
        [Column("failed_tender_full_id")] public string FailedTenderFullId{get; set;}
        [Column("contact_email")] public string ContactEmail{get; set;}
        [Column("contact_phone")] public string ContactPhone{get; set;}
        [Column("contact_requirements")] public string ContactRequirements{get; set;}
        [Column("date_start")] public DateTime? DateStart{get; set;}
        [Column("details_link")] public string DetailsLink {get; set;}
        [Column("signatory_name")] public string SignatoryName{get; set;}
        [Column("date_finish")] public DateTime? DateFinish{get; set;}
        [Column("date_opening")] public DateTime? DateOpening{get; set;}
        [Column("signature_date")] public DateTime? SignatureDate{get; set;}
        [Column("lot_amount")] public long? LotAmount{get; set;}
        [Column("full_price")] public long? FullPrice{get; set;}
        [Column("failed_tender_id")] public long? FailedTenderId{get; set;}
        [Column("customer_bin")] public long? CustomerBin{get; set;}
        [Column("konkurs_doc_name")] public string KonkursDocName{get; set;}
        [Column("konkurs_doc_link")] public string KonkursDocLink{get; set;}
        [Column("id")] public long Id{get; set;}
        public List<LotNadlocDto> Lots { get; set; }
    }
}