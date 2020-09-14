using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    /// @author Yevgeniy Cherdantsev
    /// @date 26.02.2020 18:42:59
    /// <summary>
    /// Tender Parsing DB table object
    /// </summary>
    [Table("tender_document_goszakup")]
    public class
        TenderDocumentGoszakupDto : DbLoggerCategory.Model
    {
        [Key] [Column("id")] public int Id { get; set; }
        [Column("identity")] public string Identity { get; set; }
        [Column("number")] public string Number { get; set; }
        [Column("type")] public string Type { get; set; }
        [Column("title")] public string Title { get; set; }
        [Column("link")] public string Link { get; set; }
    }
}