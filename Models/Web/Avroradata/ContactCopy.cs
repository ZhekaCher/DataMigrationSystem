using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("contacts_copy1")]
    public class ContactCopy
    {
        [Key][Column("id")] public long Id { get; set; }
        [Column("bin")] public long? Bin { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("website")] public string Website { get; set; }
        [Column("telephone")] public string Telephone { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
        
    }
}