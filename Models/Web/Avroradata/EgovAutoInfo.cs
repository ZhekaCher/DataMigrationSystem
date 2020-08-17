using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("egov_avto_info")]
    public class EgovAutoInfo
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("article_name")]
        public long ArticleName { get; set; }
        [Column("content")]
        public long Content { get; set; }
        [Column("relevance_date")] 
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
    }
    
}