using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("aisoip_companies")]
    
    public class AisoipDto
    {
       [Key]
       [Column("id")]
       public long Id { get; set; }
       
       [Column("biin")]
       public long Biin { get; set; }
       
       [Column("ares_id")]
       public long AresId { get; set; }
       
       [Column("result")]
       public bool Result { get; set; }
       
       [Column("relevance_date")]
       public DateTime? RelevanceDate { get; set; }
       
    }
    [Table("aisoip_list")]
    
    public class AisoipList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        
        [Column("id")]
        public long? Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
    }
}