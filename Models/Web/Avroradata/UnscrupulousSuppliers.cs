using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{        
    [Table("unscrupulous_suppliers")]
    public class UnscrupulousSuppliers
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("supplier_address")]
        public string SupplierAddress { get; set; }
        [Column("iin_biin")]
        public long? IinBiin { get; set; }
        [Column("buy_method")]
        public string BuyMethod { get; set; }
        [Column("final_date")]
        public DateTime? FinalDate { get; set; }
        [Column("tech_name")]
        public string TechName { get; set; }
        [Column("court")]
        public string Court { get; set; }

        [Column("relevance_date")] public DateTime RelevanceDate { get; set; } = DateTime.Now;
    }
}