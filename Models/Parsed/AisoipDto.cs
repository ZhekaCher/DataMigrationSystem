using System;
using System.Collections.Generic;
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
        public long? Biin { get; set; }
        
        [Column("result")]
        public bool Result { get; set; }
        
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        
        [Column("title")]
        public string Title { get; set; }
        public List<AisoipDetailsDto> AisoipDetailsDtos { get; set; }
     }

    [Table("aisoip_details")]
    public class AisoipDetailsDto
    {
        [Key]
        [Column("id")]
        public long Id{ get; set;}

        [Column("ares_id")]
        public long AresId { get; set; }
        
        [Column("date")]
        public DateTime Date { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("address")]
        public string Address { get; set; }
        
        [Column("tel")]
        public string Tel { get; set; }
        
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
    }
}