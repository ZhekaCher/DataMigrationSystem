using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("kaspi_auction")]
    public class KaspiAuctionDto
    { 
        [Key][Column("id")] public long? Id{get; set;} 
        [Column("number")] public string Number{get; set;}
        [Column("name")] public string Name{get; set;}
        [Column("start_cost")] public Double StartCost{get; set;}
        [Column("application_deadline")] public DateTime? ApplicationDeadline{get; set;}
        [Column("trading_start_date")] public DateTime? TradingStartDate{get; set;}
        [Column("initiator")] public string Initiator{get; set;}
        [Column("broker")] public string Broker{get; set;}
        [Column("auction")] public string Auction{get; set;}
        [Column("status_members")] public string StatusMembers{get; set;}
        [Column("best_cost")] public string BestCost{get; set;}
        [Column("doc_path")] public string DocPath{get; set;}
        [Column("source_link")] public string SourceLink{get; set;}
        [Column("bin")] public long? Bin{get; set;}
    }
}