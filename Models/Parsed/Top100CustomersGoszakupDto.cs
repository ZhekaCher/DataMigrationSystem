using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("top100customers")]
    public class Top100CustomersGoszakupDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("bin")]
        public long bin { get; set; }
        [Column("contracts")]
        public long contracts { get; set; }
        [Column("amount")]
        public string Amount { get; set; }
        [Column("place")]
        public int Place { get; set; }
        [Column("adding_date")]
        public DateTime AddingDate { get; set; }

    }
}