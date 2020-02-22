using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
	[Table("unscrupulous_goszakup")]
	public class UnscrupulousGoszakup
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.None)][Column("biin_companies")] public long? BiinCompanies{get; set;}
		[Column("status")] public bool? Status{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
	}
}
