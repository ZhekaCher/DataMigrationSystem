using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{


	/// @author Yevgeniy Cherdantsev
	/// @date 25.02.2020 09:33:46
	/// @version 1.0
	/// <summary>
	/// 'unscrupulous_goszakup' web table
	/// </summary>
	[Table("unscrupulous_goszakup")]
	public class UnscrupulousGoszakup
	{
		[Key][DatabaseGenerated(DatabaseGeneratedOption.None)][Column("biin_companies")] public long? BiinCompanies{get; set;}
		[Column("status")] public bool? Status{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
		[Column("start_date")] public DateTime? StartDate{get; set;}
	}
}
