using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("aisoip_companies")]
    public class Aisoip
    {
        [Key]
        [Column("id")]
        public long? Id { get; set; }
        
        [Column("biin")]
        public long? Biin { get; set; }
        
        [Column("ares_id")]
        public long? AresId { get; set; }
        
        [Column("result")]
        public bool Result { get; set; }
        
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
    }
    
    [Table("aisoip_list")]
    public class AisoipList
    {
        [Key]
        [Column("id")]
        public long? Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
    }
}