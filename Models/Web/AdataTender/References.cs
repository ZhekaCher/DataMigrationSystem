using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.AdataTender
{
    [Table("statuses")]
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("combined_id")]
        public long? CombinedId { get; set; }
    }
    [Table("combined_statuses")]
    public class CombinedStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("methods")]
    public class Method
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("combined_id")]
        public long? CombinedId { get; set; }
    }
    [Table("tender_priorities")]
    public class TenderPriority
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("payment_conditions")]
    public class PaymentCondition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("lot_id")]
        public long LotId { get; set; }
        [Column("final_payment")]
        public int FinalPayment { get; set; }
        [Column("interim_payment")]
        public int InterimPayment { get; set; }
        [Column("prepay_payment")]
        public int PrepayPayment { get; set; }
    }
    [Table("combined_methods")]
    public class CombinedMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("tru_codes")]
    public class TruCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("code")]
        public string Code { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("documentation_types")]
    public class DocumentationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
    [Table("sources")]
    public class Source
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("short_name")]
        public string ShortName { get; set; }
    }
}