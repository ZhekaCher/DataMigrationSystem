using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("gosreestr_bins")]
        public class GosreestrBinDto
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public long Id { get; set; }

            [Column("bin")] public long Bin { get; set; }
            [Column("gov_participation")] public double? GovParticipation { get; set; }
            public GosreestrCapitalDto GosreestrCapitalDto { get; set; }
        }
}