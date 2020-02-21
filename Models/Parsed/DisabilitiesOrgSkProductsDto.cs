using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("disabilities_organizations_products_sk")]
    public class DisabilitiesOrgSkProductsDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("products")]
        public string Products { get; set; }
        [Column("adding_date")]
        public DateTime AddingDate { get; set; }
        [Column("certificates")]
        public string Certificates { get; set; }
        [Column("positions")]
        public string Positions { get; set; }
        [Column("doc_date")]
        public DateTime DocDate { get; set; }
        [Column("validity")]
        public string Validity { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}