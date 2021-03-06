using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
	[Table("plans")]
	public class AdataPlan
	{
		[Key] [Column("id")] public int? Id{get; set;}
		[Column("source_id")] public int? SourceId{get; set;}
		[Column("source_unique_id")] public string SourceUniqueId{get; set;}
		[Column("act_number")] public string ActNumber{get; set;}
		[Column("fin_year")] public int? FinYear{get; set;}
		[Column("date_approved")] public DateTime? DateApproved{get; set;}
		[Column("status_id")] public long? StatusId{get; set;}
		[Column("name")] public string Name{get; set;}
		[Column("description")] public string Description{get; set;}
		[Column("measure_id")] public long? MeasureId{get; set;}
		[Column("count")] public double? Count{get; set;}
		[Column("price")] public double? Price{get; set;}
		[Column("amount")] public double? Amount{get; set;}
		[Column("month_id")] public int? MonthId{get; set;}
		[Column("is_qvazi")] public bool? IsQvazi{get; set;}
		[Column("tru_code")] public string TruCode{get; set;}
		[Column("date_create")] public DateTime? DateCreate{get; set;}
		[Column("extra_description")] public string ExtraDescription{get; set;}
		[Column("supply_date")] public string SupplyDate{get; set;}
		[Column("prepayment")] public double? Prepayment{get; set;}
		[Column("subject_biin")] public long? SubjectBiin{get; set;}
		[Column("method_id")] public long? MethodId{get; set;}
		[Column("contract_unit_id")]  public long? ContractUnitId { get; set; }
	}
}
