using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_unreliable_supplier")]
    public class UnreliableSk
    {
        [Column("reason")]
        private string Reason { get; set; }
        [Column("date_of_adding")]
        private DateTime AddingDate { get; set; }
        [Column("date_of_unreliable")]
        private DateTime UnreliableDate { get; set; }
        [Column("date_of_relevance")]
        private DateTime RelevanceDate { get; set; }
        [Column("biin_companies")]
        private long Biin { get; set; }
    }
}