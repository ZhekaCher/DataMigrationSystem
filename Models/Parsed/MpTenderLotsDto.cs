using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("mp_lots")]
    public class MpTenderLotsDto
    {
        [Column("id")] public long Id { get; set; }
        [Column("advert_id")] public long  AdvertId{ get; set; }
      
        [Column("lot_id")] public long LotId { get; set; }
        [Column("lot_name")] public string LotName { get; set; }
        [Column("tender_lot_owner")] public string TenderLotOwner { get; set; }
        [Column("type_of_auction")] public string TypeOfAuction { get; set; }
        [Column("lot_category")] public string LotCategory { get; set; }
        [Column("lot_region")] public string LotRegion { get; set; }
        [Column("start_auc")] public DateTime? StartAuc { get; set; }
        [Column("end_auc")] public DateTime? EndAuc { get; set; }
        [Column("initiator_of_auc")] public string InitiatorOfAuc { get; set; }
        [Column("status_of_auc")] public string StatusOfAuc { get; set; }
        [Column("opening_price")] public double? OpeningPrice { get; set; }
        [Column("amount")] public double? Amount { get; set; }
        [Column("lot_volume")] public double? LotVolume { get; set; }
        [Column("abbreviation")] public string Abbreviation { get; set; }
        [Column("min_price_step")] public double? MinPriceStep { get; set; }
        [Column("price_higher")] public string PriceHigher { get; set; }
        [Column("lot_description")] public string LotDescription { get; set; }
        [Column("requirement_suppliers")] public string RequirementSuppliers { get; set; }
        [Column("lot_access")] public string LotAccess { get; set; }
        [Column("delivery_adres")] public string DeliveryAdres { get; set; }
        [Column("pay_order")] public string PayOrder { get; set; }
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("delivery_time")] public string DeliveryTime { get; set; }
        [Column("unit_of_amount")] public string UnitOfAmount { get; set; }
        
        
        
        public List<MpTenderFilesDto> Documentations { get; set; }
    }
}