using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.TradingFloor
{
    /// @author Yevgeniy Cherdantsev
    /// @date 21.02.2020 11:54:37
    /// @version 1.0
    /// <summary>
    /// 'strading_floor' web table
    /// </summary>
    
    [Table("strading_floor")]
    public class STradingFloor : BaseReferenceEntity
    {
        [Column("code")]
        public string Code { get; set; }
    }
}