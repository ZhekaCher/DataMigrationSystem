using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("esauda_tender")]
    public class EsaudaTenderDto
    {
        [Key] [Column("id")] public long Id{get; set;}
        [Column("title_ru")] public string TitleRu{get; set;}
        [Column("auction_id")] public long AuctionId{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("path")] public string Path{get; set;}
        [Column("title_kz")] public string TitleKz{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}
        [Column("start_sum")] public string StartSum{get; set;}
        [Column("step_sum")] public string StepSum{get; set;}
        [Column("bin")] public long Bin{get; set;}
        [Column("company_name")] public string CompanyName{get; set;}
        [Column("cat_name")] public string CatName{get; set;}
        [Column("buy_status")] public string BuyStatus{get; set;}
        [Column("buy_status_kz")] public string BuyStatusKz{get; set;}
        [Column("start_date")] public DateTime StartDate{get; set;}
        [Column("end_datetime")] public DateTime EndDatetime{get; set;}
    }
}