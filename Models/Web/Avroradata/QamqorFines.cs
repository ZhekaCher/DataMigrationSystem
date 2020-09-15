using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("qamqor_fines")]
    public class QamqorFines
    {
        [Key] [Column("id")]  public long Id { get; set; }
        [Column("num_administrative")]  public string NumAdministrative { get; set; }
        [Column("consideration_date")]  public DateTime? ConsiderationDate { get; set; }
        [Column("commission_date")]  public DateTime? CommissionDate { get; set; }
        [Column("authority")]  public string Authority { get; set; }
        [Column("main_measure")]  public string MainMeasure { get; set; }
        [Column("administrative_fine")]  public string AdministrativeFine { get; set; }
        [Column("fine_status")]  public bool FineStatus { get; set; }
        [Column("gos_number")]  public string GosNumber { get; set; }
    }
}