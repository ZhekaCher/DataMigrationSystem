using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("nationalbank_lot")]
    public class NationalBankTenderLotDto
    {
        [Column("id")] public long Id { get; set; }
        [Column("advert_id")] public string AdvertId { get; set; }
        [Column("lot_id")] public string LotId { get; set; }
        [Column("lot_name_ru")] public string LotNameRu { get; set; }
        [Column("lot_name_kz")] public string LotNameKz { get; set; }
        [Column("lot_tru")] public string LotTru { get; set; }
        [Column("lot_description_ru")] public string LotDescriptionRu { get; set; }
        [Column("lot_description_kz")] public string LotDescriptionKz { get; set; }
        [Column("lot_characteristic_ru")] public string LotCharacteristicRu { get; set; }
        [Column("lot_characteristic_kz")] public string LotCharacteristicKz { get; set; }
        [Column("lot_year")] public string LotYear { get; set; }
        [Column("lot_period")] public string LotPeriod { get; set; }
        [Column("lot_sum")] public string LotSum { get; set; }
        [Column("lot_customer_name")] public string LotCustomerName { get; set; }
        [Column("relevance_time")] public DateTime RelevanceTime { get; set; }
        [Column("lot_volume")] public double? LotVolume { get; set; }
        [Column("lot_amount")] public double? LotAmount { get; set; }
        [Column("price_per_unit")] public double? PricePerUnit { get; set; }
        [Column("unit_of_measure")] public string UnitOfMeasure { get; set; }
        [Column("source_link")] public string SourceLink { get; set; }
        [Column("delivery_address")] public string DeliveryAddress { get; set; }
        
        public List<NationalBankFileDto> Documentations { get; set; }
    }
}