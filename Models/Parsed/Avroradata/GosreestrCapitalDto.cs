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
        [Column("capital")] public string Capital { get; set; }
        [Column("gov_contribution")] public string GovContribution { get; set; }
        [Column("gov_participation")] public string GovParticipation { get; set; }
        [Column("gov_package")] public string GovPackage { get; set; }
        [Column("registrar")] public string Registrar { get; set; }
        [Column("free_contribution")] public string FreeContribution { get; set; }
        [Column("free_shares")] public string FreeShares { get; set; }
        [Column("shares_encumbered")] public string SharesEncumbered { get; set; }
    }
}