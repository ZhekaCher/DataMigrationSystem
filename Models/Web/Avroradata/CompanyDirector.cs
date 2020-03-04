using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
	[Table("company_director")]
	public class CompanyDirector{
		[Column("company_bin")] public long? CompanyBin{get; set;}
		[Column("director_iin")] public long? DirectorIin{get; set;}
		[Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
		[Column("i_datasource")] public long? DataSource{get; set;}
		[Column("date_begin")] public DateTime? DateBegin{get; set;}
		[Column("date_end")] public DateTime? DateEnd{get; set;}
		
	}
}
