using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{
    [Table("lots")]
    public class AdataLot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("source_number")]
        public string SourceNumber { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("announcement_id")]
        public long AnnouncementId { get; set; }
        [Column("status_id")]
        public long? StatusId { get; set; }
        [Column("method_id")]
        public long? MethodId { get; set; }
        [Column("source_id")]
        public long SourceId { get; set; }
        [Column("application_start_date")]
        public DateTime? ApplicationStartDate { get; set; }
        [Column("application_finish_date")]
        public DateTime? ApplicationFinishDate { get; set; }
        [Column("customer_bin")]
        public long? CustomerBin { get; set; }
        [Column("supply_location")]
        public string SupplyLocation { get; set; }
        [Column("tender_location")]
        public string TenderLocation { get; set; }
        [Column("tru_id")]
        public long? TruId { get; set; }
        [Column("characteristics")]
        public string Characteristics { get; set; }
        [Column("quantity")]
        public double Quantity { get; set; }
        [Column("measure_id")]
        public long? MeasureId { get; set; }
        [Column("unit_price")]
        public double UnitPrice { get; set; }
        [Column("total_amount")]
        public double TotalAmount { get; set; }
        [Column("terms")]
        public string Terms { get; set; }
        [Column("source_link")]
        public string SourceLink { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
        [Column("flag_prequalification")]
        public bool FlagPrequalification { get; set; }  
        public List<LotDocumentation> Documentations { get; set; } =  new List<LotDocumentation>();
    }

    [Table("lot_documentations")]
    public class LotDocumentation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("documentation_type_id")]
        public long? DocumentationTypeId { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
        [Column("lot_id")]
        public long LotId { get; set; }
        [Column("source_link")]
        public string SourceLink { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}