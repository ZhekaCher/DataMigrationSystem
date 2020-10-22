namespace DataMigrationSystem.Models.Parsed.Avroradata.EtbTender
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations.Schema;
	
	[Table("etb_trade_tender")]
	public class EtbTenderDto
	{
		[Column("id_group")] public long IdGroup { get; set; }

		[Column("id_group_full")] public string IdGroupFull { get; set; }

		[Column("name_group_lots")] public string NameGroupLots { get; set; }

		[Column("total_no_taxes")] public double TotalNoTaxes { get; set; }

		[Column("lots_in_group")] public long LotsInGroup { get; set; }

		[Column("date_published")] public DateTime DatePublished { get; set; }

		[Column("date_fin_accepting")] public DateTime DateFinAccepting { get; set; }

		[Column("ordering_company")] public string OrderingCompany { get; set; }

		[Column("company_address")] public string CompanyAddress { get; set; } // NEW

		[Column("contact_name")] public string ContactName { get; set; } // NEW

		[Column("contact_mail")] public string ContactMail { get; set; } // NEW

		[Column("contact_phone")] public string ContactPhone { get; set; } // NEW

		[Column("status")] public string Status { get; set; }

		[Column("completion_strategy")] public string CompletionStrategy { get; set; }

		public List<EtbLotDto> EtbLots { get; set; }
	}
}