using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("ets_announcements")]
        public class AnnouncementEtsTenderDto
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public long Id { get; set; }

            [Column("payment_conditions")] public string PaymentConditions { get; set; }
            [Column("full_price")] public double FullPrice { get; set; }
            [Column("delivery_conditions")] public string DeliveryConditions { get; set; }
            [Column("contact_person")] public string ContactPerson { get; set; }
            [Column("finish_date")] public DateTime? FinishDate { get; set; }
            [Column("amount")] public string Amount { get; set; }
            [Column("purchase_type")] public string PurchaseType { get; set; }
            [Column("currency")] public string Currency { get; set; }
            [Column("start_date")] public DateTime? StartDate { get; set; }
            [Column("source_code")] public string SourceCode { get; set; }
            [Column("delivery_place")] public string DeliveryPlace { get; set; }
            [Column("customer")] public string Customer { get; set; }
            [Column("title")] public string Title { get; set; }
            [Column("last_update_date")] public DateTime? LastUpdateDate { get; set; }
            [Column("unit_price")] public string UnitPrice { get; set; }
            [Column("proposals_possibility")] public string ProposalsPossibility { get; set; }
            [Column("security_contract")] public string SecurityContract { get; set; }
            [Column("min_step")] public string MinStep { get; set; }
            [Column("automatic_renewal")] public string AutomaticRenewal { get; set; }
            [Column("signature_information")] public string SignatureInformation { get; set; }
            [Column("alternative_applications")] public string AlternativeApplications { get; set; }
            [Column("higher_price_prohibition")] public string HigherPriceProhibition { get; set; }
            [Column("procedure_place")] public string ProcedurePlace { get; set; }
            [Column("doc_required")] public string DocRequired { get; set; }
            [Column("hide_price")] public string HidePrice { get; set; }
            [Column("status")] public bool Status { get; set; }
            [Column("customer_bin")] public long? CustomerBin { get; set; }
            [Column("source_link")] public string SourceLink { get; set; }
            [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
            [Column("rubrics")] public string Rubrics { get; set; }
            
            public List<LotEtsTenderDto> Lots { get; set; }
            public List<PurchasingPositionsEtsTenderDto> Positions { get; set; }
        }

        [Table("ets_lots")]
        public class LotEtsTenderDto
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public long Id { get; set; }

            [Column("lot_number")] public string LotNumber { get; set; }
            [Column("lot_name")] public string LotName { get; set; }
            [Column("full_price")] public double FullPrice { get; set; }
            [Column("source_code")] public string SourceCode { get; set; }
        }

        [Table("ets_purchasing_positions")]
        public class PurchasingPositionsEtsTenderDto
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public long Id { get; set; }

            [Column("lot_number")] public string LotNumber { get; set; }
            [Column("total_limit_value")] public string TotalLimitValue { get; set; }
            [Column("source_code")] public string SourceCode { get; set; }
            [Column("unit")] public string Unit { get; set; }
            [Column("count")] public string Count { get; set; }
            [Column("unit_price")] public string UnitPrice { get; set; }
            [Column("name")] public string Name { get; set; }
            
        }
    
}