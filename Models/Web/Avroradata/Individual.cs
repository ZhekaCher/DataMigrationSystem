using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("individuals")]
    public class Individual
    {
        [Key]
        [Column("iin")]
        public long Iin { get; set; }
    }
    [Table("individual")]
    public class ParsedIndividual
    {
        [Key]
        [Column("iin")]
        public long Iin { get; set; }
    }
}