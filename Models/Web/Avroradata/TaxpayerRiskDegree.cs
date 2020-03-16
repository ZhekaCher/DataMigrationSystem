using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("taxpayer_risk_degree")]
    public class TaxpayerRiskDegree
    {
        [Column("id")] public long? Id{get; set;}
        [Column("bin")] public long? Bin{get; set;}
        [Column("degree")] public string Degree{get; set;}
        [Column("calculation_relevance_date")] public DateTime? CalculationRelevanceDate{get; set;}
        [Column("relevance_date_from")] public DateTime? RelevanceDateFrom{get; set;}
        [Column("relevance_date_to")] public DateTime? RelevanceDateTo{get; set;}
		
    }
}