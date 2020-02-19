using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("forced_liquidated_company")]
    public class ForcedLiquidationCompanyDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("bin")]
        public long Bin { get; set; }
        [Column("rnn")]
        public long Rnn { get; set; }
        [Column("kno")]
        public long Kno { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("authority_name")]
        public string AuthorityName { get; set; }
        [Column("authority_address")]
        public string AuthorityAddress { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}