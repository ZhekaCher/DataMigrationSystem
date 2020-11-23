using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Parsing
{
        [Table("relevance_dates")]
        public class RelevanceDate
        {
            [Key] [Column("id")] public long? Id{get; set;}
            [Column("code")] public string Code{get; set;}
            [Column("relevance")] public DateTime? Relevance{get; set;}
        }
}