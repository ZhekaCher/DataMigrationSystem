using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("top100suppliers")]
    public class Top100SuppliersGoszakupDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("contracts")]
        public long Contracts { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
        [Column("place")]
        public int Place { get; set; }
        [Column("adding_date")]
        public DateTime? AddingDate { get; set; }
    }
}