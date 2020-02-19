using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{

    /// @author Yevgeniy Cherdantsev
    /// @date 19.02.2020 17:44:50
    /// @version 1.0
    /// <summary>
    /// 'lots_goszakup' parse table
    /// </summary>
    [Table("lots_goszakup")]
    public class ParsedLotsGoszakup
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("lot_number")]
        public string LotNumber { get; set; }
        [Column("ref_lot_status_id")]
        public int RefLotStatusId { get; set; }
        [Column("last_update_date")]
        public DateTime LastUpdateDate { get; set; }
        [Column("union_lots")]
        public bool UnionLots { get; set; }
        [Column("count")]
        public double Count { get; set; }
        [Column("amount")]
        public double Amount { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("name_kz")]
        public string NameKz { get; set; }
        [Column("description_ru")]
        public string DescriptionRu { get; set; }
        [Column("description_kz")]
        public string DescriptionKz { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        [Column("customer_bin")]
        public long CustomerBin { get; set; }
        [Column("trd_buy_number_anno")]
        public string TrdBuyNumberAnno { get; set; }
        [Column("trd_buy_id")]
        public int TrdBuyId { get; set; }
        [Column("dumping")]
        public bool Dumping { get; set; }
        [Column("dumping_lot_price")]
        public int DumpingLotPrice { get; set; }
        [Column("psd_sign")]
        public byte PsdSign { get; set; }
        [Column("consulting_services")]
        public bool ConsultingServices { get; set; }
        [Column("single_org_sign")]
        public byte SingleOrgSign { get; set; }
        [Column("is_light_industry")]
        public bool IsLightIndustry { get; set; }
        [Column("is_construction_work")]
        public bool IsConstructionWork { get; set; }
        [Column("disable_person_id")]
        public bool DisablePersonId { get; set; }
        [Column("customer_name_kz")]
        public string CustomerNameKz { get; set; }
        [Column("customer_name_ru")]
        public string CustomerNameRu { get; set; }
        [Column("ref_trade_methods_id")]
        public int RefTradeMethodsId { get; set; }
        [Column("index_date")]
        public DateTime IndexDate { get; set; }
        [Column("system_id")]
        public int SystemId { get; set; }
        
    }
}