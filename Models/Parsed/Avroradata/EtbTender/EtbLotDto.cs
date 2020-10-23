namespace DataMigrationSystem.Models.Parsed.Avroradata.EtbTender
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    
    [Table("etb_trade_lot")]
    public class EtbLotDto
    {
        [Column("id_lot")]
        public long IdLot { get; set; }

        [Column("id_group")]
        public long IdGroup { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("short_description")]
        public string ShortDescription { get; set; }

        [Column("measurement_units")]
        public string MeasurementUnits { get; set; }

        [Column("volume")]
        public double Volume { get; set; }

        [Column("unit_total_no_tax")]
        public double? UnitTotalNoTax { get; set; }

        [Column("group_total_no_tax")]
        public double GroupTotalNoTax { get; set; }

        [Column("completion_period")]
        public string CompletionPeriod { get; set; }

        [Column("completion_condition")]
        public string CompletionCondition { get; set; }

        [Column("payment_condition")]
        public string PaymentCondition { get; set; }

        // Some weird ДКС (%)
        [Column("dkc")]
        public double? Dkc { get; set; }

        [Column("completion_rate")]
        public double CompletionRate { get; set; }

        [Column("n_orders")]
        public double? NOrders { get; set; }

        public ICollection<EtbDetailDto> EtbDetails { get; set; }
    }
}