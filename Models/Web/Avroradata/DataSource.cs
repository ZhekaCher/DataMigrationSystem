using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
	[Table("data_source")]
	public class DataSource
	{
		[Key] [Column("i")] public long? I{get; set;}
		[Column("name")] public string Name{get; set;}
		[Column("code")] public string Code{get; set;}
		
	}
}
