using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
   
    [Table("kompra_bin_parser")]
    public class KompraBinParserDto
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("iin_bin")] public long IinBin { get; set; }
        [Column("rnn")] public long? Rnn { get; set; }
        [Column("address")] public string Address { get; set; }
        [Column("owner")] public string Owner { get; set; }
        [Column("ownership_type")] public string OwnershipType { get; set; }
        [Column("kato")] public long? Kato { get; set; }
        [Column("workers")] public string Workers { get; set; }
        [Column("okpo")] public long? Okpo { get; set; }
        [Column("oked")] public long? Oked { get; set; }
        [Column("secondary_oked")] public long? SecondaryOked { get; set; }
        [Column("kbe")] public long? Kbe { get; set; }
        [Column("status")] public bool? Status { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
    }
}