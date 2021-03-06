﻿using System;
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
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
    }
    
    [Table("aisoip_list")]
    public class AisoipList
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
    }
    
    [Table("aisoip_details")]
    public class AisoipDetails
    {
        [Key]
        [Column("id")]
        public long Id{ get; set;}

        [Column("ares_id")]
        public long AresId { get; set; }
        
        [Column("biin")]
        public long? Biin { get; set; }
        
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