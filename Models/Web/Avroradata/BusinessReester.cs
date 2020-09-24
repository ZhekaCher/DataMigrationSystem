using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("business_reester_property_form")]
    public class BusinessReester
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("region")] public string Region { get; set; }
        [Column("district")] public string District { get; set; }
        [Column("locality")] public string Locality { get; set; }
        [Column("industry")] public string Industry { get; set; }
        [Column("activity")] public string Activity { get; set; }
        [Column("legal_address")] public string LegalAddress { get; set; }
        [Column("actual_address")] public string ActualAddress { get; set; }
        [Column("organizational_form")] public string OrganizationalForm { get; set; }
        [Column("ownership_type")] public string OwnershipType { get; set; }
        [Column("bin")] public long? Bin { get; set; }
        [Column("established_year")] public string EstablishedYear { get; set; }
        [Column("status_payer")] public bool? Status { get; set; }
        [Column("cluster")] public string Cluster { get; set; }
        [Column("owner")] public string Owner { get; set; }
    }
}