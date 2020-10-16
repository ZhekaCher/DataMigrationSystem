using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("contacts_telephone")]
    public class ContactTelephone
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("biin")] public long? Bin { get; set; }
        [Column("telephone")] public string Telephone { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
    }
    [Table("contacts_website")]
    public class ContactWebsite
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("biin")] public long? Bin { get; set; }
        [Column("website")] public string Website { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
    }
    [Table("contacts_email")]
    public class ContactEmail
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("biin")] public long? Bin { get; set; }
        [Column("email")] public string Email { get; set; }
        [Column("source")] public string Source { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
    }
}