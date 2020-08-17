using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("egov_avto_info")]
    public class EgovAutoInfoDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("article_name")]
        public string ArticleName { get; set; }
        [Column("content")]
        public string Content { get; set; }
    }
}