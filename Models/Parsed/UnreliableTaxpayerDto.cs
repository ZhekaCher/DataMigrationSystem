using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    
    [Table("unreliable_taxpayers")]
    public class UnreliableTaxpayerDto
    {
        [Key] 
        [Column("id")] public long? Id{get; set;}
        [Column("iin_bin")] public long? BinCompany{get; set;}
        [Column("rnn")] public long? RnnCompany{get; set;}
        [Column("name_company")] public string NameCompany{get; set;}
        [Column("name_head")] public string NameHead{get; set;}
        [Column("iin_head")] public long? IinHead{get; set;}
        [Column("rnn_head")] public long? RnnHead{get; set;}
        [Column("document_number")] public string DocumentNumber{get; set;}
        [Column("document_date")] public DateTime? DocumentDate{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        [Column("id_type_document")] public long? IdTypeDocument{get; set;}
        [Column("id_list_type")] public long? IdListType{get; set;}
        [Column("note")] public string Note{get; set;}
    }
}