using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{
    [Table("statuses")]
    public class Status
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("combined_id")]
        public long CombinedId { get; set; }
    }
    [Table("combined_statuses")]
    public class CombinedStatus
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("methods")]
    public class Method
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("combined_id")]
        public long CombinedId { get; set; }
    }
    [Table("combined_methods")]
    public class CombinedMethod
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("short_name")]
        public string ShortName { get; set; }
    }
    [Table("measures")]
    public class Measure
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("tru_codes")]
    public class TruCode
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("documentation_types")]
    public class DocumentationType
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("sources")]
    public class Source
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("short_name")]
        public string ShortName { get; set; }
    }
}