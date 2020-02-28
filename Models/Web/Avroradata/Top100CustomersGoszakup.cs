using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("goszakup_top_100_customer")]
    public class Top100CustomersGoszakup
    {
        [Column("number_of_contracts")]
        public long Contracts { get; set; }
        [Column("amount")]
        public string Amount { get; set; }
        [Column("place")]
        public int Place { get; set; }
        [Column("date_of_adding")] public DateTime AddingDate { get; set; }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("biin_companies")]
        public long Bin { get; set; }

    }
}