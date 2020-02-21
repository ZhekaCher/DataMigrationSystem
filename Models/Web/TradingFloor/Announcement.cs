using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 13:02:57
    /// @version 1.0
    /// <summary>
    /// 'announcement' web table
    /// </summary>
    /// 
    [Table("announcement")]
    public class Announcement
    {
        [Key]
        [Column("id")]
        public long? Id { get; set; }
        [Column("id_anno")]
        public long? IdAnno { get; set; }
        [Column("id_tf")]
        public long? IdTf { get; set; }
        [Column("number_anno")]
        public string NumberAnno { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("name_kz")]
        public string NameKz { get; set; }
        [Column("name_en")]
        public string NameEn { get; set; }
        [Column("dt_start")]
        public DateTime? DtStart { get; set; }
        [Column("dt_end")]
        public DateTime? DtEnd { get; set; }
        [Column("customer_bin")]
        public long? CustomerBin { get; set; }
        [Column("organizer_bin")]
        public long? OrganizerBin { get; set; }
        //TODO(token field)
        // [Column("token")]
        // public long? Token { get; set; }
        //TODO(contact_info field)
        // [Column("contact_info")]
        // public string ContactInfo { get; set; }
        [Column("id_tender_type")]
        public long? IdTenderType { get; set; }
        //TODO(add_info field)
        // [Column("add_info")]
        // public string AddInfo { get; set; }
        [Column("sum_tru_no_nds")]
        public double? SumTruNoNds { get; set; }
        [Column("parent_id")]
        public long? ParentId { get; set; }
        [Column("relevance_date")]
        public DateTime? RelevanceDate { get; set; }
    }
}