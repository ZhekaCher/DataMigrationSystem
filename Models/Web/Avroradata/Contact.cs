using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("contacts")]
    public class Contact
    {
        [Column("bin")] public long? Bin { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("website")] public string Website { get; set; }
        [Column("telephone")] public string Telephone { get; set; }
        [Column("source")] public string Source { get; set; }
    }
}