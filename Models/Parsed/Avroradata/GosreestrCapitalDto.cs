using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("gosreestr_authorized_capitals")]
    public class GosreestrCapitalDto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("bin")] public long Bin { get; set; }
        [Column("capital")] public double? Capital { get; set; }
        [Column("gov_contribution")] public double? GovContribution { get; set; }
        [Column("gov_participation")] public double? GovParticipation { get; set; }
        [Column("gov_package")] public long? GovPackage { get; set; }
        [Column("registrar")] public string Registrar { get; set; }
        [Column("free_contribution")] public double? FreeContribution { get; set; }
        [Column("free_shares")] public long? FreeShares { get; set; }
        [Column("shares_encumbered")] public long? SharesEncumbered { get; set; }
    }
}