using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{
    [Table("announcements")]
    public class AdataAnnouncement
    {

        [Column("id")]
        public long Id { get; set; }
        [Column("source_number")]
        public string SourceNumber { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("status_id")]
        public long StatusId { get; set; }
        [Column("method_id")]
        public long MethodId { get; set; }
        [Column("source_id")]
        public long SourceId { get; set; }
        [Column("application_start_date")]
        public DateTime? ApplicationStartDate { get; set; }
        [Column("application_finish_date")]
        public DateTime? ApplicationFinishDate { get; set; }
        [Column("customer_bin")]
        public long? CustomerBin { get; set; }
        [Column("source_link")]
        public string SourceLink { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
        [Column("lots_quantity")]
        public long LotsQuantity { get; set; }
        [Column("lots_amount")]
        public double LotsAmount { get; set; }
        [NotMapped]
        public List<AdataLot> Lots = new List<AdataLot>();
    }
    [Table("announcement_documentations")]
    public class AnnouncementDocumentation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("documentation_type_id")]
        public long DocumentationTypeId { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; } = DateTime.Now;
        [Column("announcement_id")]
        public long AnnouncementId { get; set; }
    }

    [Table("announcement_contacts")]
    public class AnnouncementContact
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("email_address")]
        public string EmailAddress { get; set; }
        [Column("phone_number")]
        public string PhoneNumber { get; set; }
        [Column("announcement_id")]
        public long AnnouncementId { get; set; }
    }
}