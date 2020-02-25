using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_unreliable_supplier")]
    public class UnreliableSk
    {
        [Column("reason")]
        public string Reason { get; set; }
        [Column("date_of_adding")]
        public DateTime AddingDate { get; set; }
        [Column("date_of_unreliable")]
        public DateTime UnreliableDate { get; set; }
        [Column("date_of_relevance")]
        public DateTime RelevanceDate { get; set; }
        [Column("biin_companies")]
        public long Biin { get; set; }
    }
}