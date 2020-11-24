using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("kgd_searched_taxpayers")]
    public class KgdSearchedTaxpayersDto
    {
        [Key] [Column("id")] public int? Id{get; set;}
        [Column("iin_bin")] public long? IinBin{get; set;}
        [Column("org_name")] public string OrgName{get; set;}
        [Column("number")] public string Number{get; set;}
        [Column("preparation_date")] public DateTime? PreparationDate{get; set;}
        [Column("posted_date")] public DateTime? PostedDate{get; set;}
    }
}