using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("company_bin")]

    public class IndividualIin
    {
        [Key] [Column("code")] public long? Code { get; set; }
    }
}