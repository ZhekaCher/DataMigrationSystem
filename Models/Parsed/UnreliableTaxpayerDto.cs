using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    
    [Table("unreliable_taxpayer")]
    public class UnreliableTaxpayerDto
    {
        [Key] 
        [Column("i")] public long? Id{get; set;}
        [Column("bin_company")] public long? BinCompany{get; set;}
        [Column("rnn_company")] public long? RnnCompany{get; set;}
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