using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("gosreestr_bins")]
        public class GosreestrBin
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Column("id")]
            public long Id { get; set; }

            [Column("bin")] public long Bin { get; set; }
            [Column("gov_participation")] public double? GovParticipation { get; set; }
            public GosreestrCapital GosreestrCapital { get; set; }
        }
}