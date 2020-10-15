using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("business_reester_property_form")]
    public class BusinessReester
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("ownership_type_id")] public long? OwnershipTypeId { get; set; }
        [Column("bin")] public long? Bin { get; set; }
    }
    [Table("business_reester_ownership_type")]
    public class BusinessReesterOwnershipType : BaseReferenceEntity
    {
    }
}