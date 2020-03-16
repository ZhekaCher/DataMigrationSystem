using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("samruk_all_participants")]
    public class SamrukParticipants
    {
        [Key]
        [Column("id")] public long? Id { get; set; }
        [Column("bin")] public long? CodeBin { get; set; }
        [Column("director_fullname")] public string DirectorFullname { get; set; }
        [Column("director_iin")] public long? DirectorIin { get; set; }
        [Column("customer")] public bool? Customer { get; set; }
        [Column("supplier")] public bool? Supplier { get; set; }
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
    }
}