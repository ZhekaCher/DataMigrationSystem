using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("top_100_customer")]
    public class Top100CustomersGoszakup
    {
        [Column("number_of_contracts")]
        private string Contracts { get; set; }
        [Column("amount")]
        private long Amount { get; set; }
        [Column("place")]
        private int Place { get; set; }
        [Column("date_of_adding")] 
        private DateTime AddingDate { get; set; }
        [Column("biin_companies")]
        private long Biin { get; set; }

    }
}