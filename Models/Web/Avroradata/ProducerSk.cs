using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_producer_company")]
    public class ProducersSk
    {
        [Key] [Column("i")] public long Id { get; set; }
        
        [Column("ceo")] public string Ceo { get; set; }
        
        [Column("contacts")] public string Contacts { get; set; }
        [Column("bin")] public  long Bin { get; set; }
        [Column("producer_type")] public int ProducerType { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
    }
    
    [Table("samruk_producer_company_products")]
    public class ProducerProducts
    {
        [Key]
        [Column("i")] public long? I{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("products")] public string Products{get; set;}
        [Column("adding_date")] public DateTime? AddingDate{get; set;}
        [Column("certificates")] public string Certificates{get; set;}
        [Column("positions")] public string Positions{get; set;}
        [Column("doc_date")] public DateTime? DocDate{get; set;}
        [Column("validity")] public string Validity{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
		
    }
    
}