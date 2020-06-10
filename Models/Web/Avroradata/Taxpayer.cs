using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("taxpayer")]
    public class Taxpayer
    {
        [Column("rnn")] public long? Rnn{get; set;}
        [Column("full_name")] public string FullName{get; set;}
        [Column("uin")] public long? Uin{get; set;}
        [Column("date_reg")] public DateTime? DateReg{get; set;}
        [Column("date_unreg")] public DateTime? DateUnreg{get; set;}
        [Column("reason_unreg")] public string ReasonUnreg{get; set;}
        [Column("add_info")] public string AddInfo{get; set;}
        [Column("period")] public string Period{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("type_id")] public int? TypeId{get; set;}
    }
    
    [Table("type_of_service")]
    public class TypeOfService
    {
        [Key] [Column("i")] public long? I{get; set;}
        [Column("name")] public string Name{get; set;}
    }
}