using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("gos_reester")]
    public class GosReesterDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("reg_num")]
        public string RegNum { get; set; }
        
        [Column("reg_date")]
        public DateTime? RegDate { get; set; }
        
        [Column("status")]
        public string Status { get; set; }
        
        [Column("owner")]
        public string Owner { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("mktu_text")]
        public string MktuText { get; set; }
        
        [Column("validation_date")]
        public DateTime? ValidationDate { get; set; }
        
        [Column("newsletter_num")]
        public string NewsletterNum { get; set; }
        
        [Column("newsletter_date")]
        public DateTime? NewsletterDate { get; set; }

        [Column("relevance_date")] public DateTime RelevanceDate { get; set; } = DateTime.Now;

    }
}