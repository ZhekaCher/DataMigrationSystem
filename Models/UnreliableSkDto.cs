using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("unreliable_suppliers_sk")]
    public class UnreliableSkDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("reason")]
        public string Reason { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("unreliable_date")]
        public DateTime UnreliableDate { get; set; }
        [Column("founder_date")]
        public string FounderData { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("adding_date")]
        public DateTime AddingDate { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}