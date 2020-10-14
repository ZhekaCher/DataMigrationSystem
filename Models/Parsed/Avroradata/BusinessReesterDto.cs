using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("business_reester_property_form")]
    public class BusinessReesterDto
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("ownership_type")] public string OwnershipType { get; set; }
        [Column("bin")] public long? Bin { get; set; }
        
    }
}