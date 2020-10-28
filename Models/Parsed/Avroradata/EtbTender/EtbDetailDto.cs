using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata.EtbTender
{
    [Table("etb_trade_lot_detalization")]
    public class EtbDetailDto
    {
        [Column("id_lot")]
        public long IdLot { get; set; }

        [Column("id_group")]
        public long IdGroup { get; set; }

        [Column("id_detail")]
        public long IdDetail { get; set; }

        [Column("detail_name")]
        public string DetailName { get; set; }

        [Column("detail_volume")]
        public double DetailVolume { get; set; }

        [Column("detail_p_unit_no_tax")]
        public double DetailPUnitNoTax { get; set; }

        [Column("detail_m_units")]
        public string DetailMUnits { get; set; }

        [Column("detail_total_no_tax")]
        public double DetailTotalNoTax { get; set; }

        [Column("tech_description")]
        public string TechDescription { get; set; }

        // Some weird ДКС (%)
        [Column("dkc")]
        public double Dkc { get; set; }

        [Column("deadline_agreement")]
        public string DeadlineAgreement { get; set; }

        [Column("delivery_address")]
        public string DeliveryAddress { get; set; }

        [Column("payment_agreement")]
        public string PaymentAgreement { get; set; }
    }
}