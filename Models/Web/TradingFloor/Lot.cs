using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 17:44:59
    /// @version 1.0
    /// <summary>
    /// 'lots' web table
    /// </summary>
    [Table("lot")]
    public class Lot
    {
        [Key]
        [Column("id")]
        public long? Id { get; set; }
        [Column("id_lot")]
        public long? IdLot { get; set; }
        [Column("id_anno")]
        public long? IdAnno { get; set; }
        [Column("id_tf")]
        public long? IdTf { get; set; }
        [Column("number_lot")]
        public string NumberLot { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("name_kz")]
        public string NameKz { get; set; }
        [Column("name_en")]
        public string NameEn { get; set; }
        [Column("quantity")]
        public double? Quantity { get; set; }
        [Column("price")]
        public double? Price { get; set; }
        [Column("total")]
        public double? Total { get; set; }
        [Column("bin_organization")]
        public long? BinOrganization { get; set; }
        [Column("customer_bin")]
        public long? CustomerBin { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
        [Column("tender_location")]
        public string TenderLocation { get; set; }
        [Column("delivery_address")]
        public string DeliveryAddress { get; set; }
        [Column("biin_supplies")]
        public long? BiinSupplies { get; set; }
        [Column("description_ru")]
        public string DescriptionRu { get; set; }
        [Column("description_kz")]
        public string DescriptionKz { get; set; }
    }
    
}