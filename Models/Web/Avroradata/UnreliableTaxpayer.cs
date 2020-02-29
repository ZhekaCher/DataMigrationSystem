using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("unreliable_taxpayers")]
    public class UnreliableTaxpayer
    {
        [Key]
        [Column("id")] public long Id {get; set;}
        [Column("bin_company")] public long? BinCompany{get; set;}
        [Column("document_number")] public string DocumentNumber{get; set;}
        [Column("document_date")] public DateTime? DocumentDate{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("id_type_document")] public long? IdTypeDocument{get; set;}
        [Column("id_list_type")] public long? IdListType{get; set;}
        [Column("note")] public string Note{get; set;}
    }
}