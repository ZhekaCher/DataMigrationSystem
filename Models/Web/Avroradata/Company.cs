using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("companies")]
    public class Company
    {
        [Key]
        [Column("biin")]
        public long Bin { get; set; }
    }
    [Table("company")]
    public class ParsedCompany
    {
        [Key]
        [Column("iin_bin")]
        public long Bin { get; set; }
    }
}