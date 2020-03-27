using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("tax_details")]
    public class TaxDetails
    {
        [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("year")] public int? Year{get; set;}
        [Column("amount")] public long? Amount{get; set;}
		
    }
}