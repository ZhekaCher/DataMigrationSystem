using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{
    [Table("companies")]
    public class Company
    {
        [Key]
        [Column("biin")]
        public long Bin { get; set; }
    }

}