using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
	[Table("nds_detail")]

	public class NdsDetailsDto
	{
		[Column("id")] public long? Id { get; set; }
		[Column("rnn")] public long? Rnn { get; set; }
		[Column("bin")] public long? Bin { get; set; }
		[Column("start_date")] public DateTime? StartDate { get; set; }
		[Column("finish_date")] public DateTime? FinishDate { get; set; }
		[Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
		[Column("reason")] public string Reason { get; set; }
		[Column("head_name")] public string HeadName { get; set; }
		[Column("head_iin_bin")] public string HeadIinBin { get; set; }
		[Column("head_rnn")] public string HeadRnn { get; set; }
	}
}