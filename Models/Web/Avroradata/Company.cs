using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("companies")]
    public class Company
    {
        [Key]
        [Column("biin")] 
        public long Bin { get; set; }
        [Column("id_region")] 
        public long? IdRegion{get; set;}
        [Column("rnn")] public long Rnn{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}
        [Column("date_registration")] public DateTime? DateRegistration{get; set;}
        [Column("id_krp")] public int IdKrp{get; set;}
        [Column("id_kato")] public int IdKato{get; set;}
        [Column("legal_address")] public string LegalAddress{get; set;}
        [Column("fullname_director")] public string FullnameDirector{get; set;}
        [Column("fullname_founders")] public string FullnameFounders{get; set;}
        [Column("relevance_date")] public DateTime? RelevanceDate{get; set;}
        public List<CompanyOked> CompanyOkeds { get; set; }
    }
    [Table("krp")]
    public class Krp
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] [Column("i")] public byte? I{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}
    }
    [Table("kato")]
    public class Kato
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] [Column("i")] public byte? I{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}
    }
    [Table("region_list")]
    public class RegionList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] [Column("i")] public long I{get; set;}
        [Column("download_code")] public string DownloadCode{get; set;}
        [Column("name_ru")] public string NameRu{get; set;}
        [Column("name_kz")] public string NameKz{get; set;}

    }
    [Table("companies_oked")]
    public class CompanyOked
    {
        [Column("c_id")]
        public long CompanyId { get; set; }
        [Column("o_id")]
        public string OkedId { get; set; }
        [Column("type")]
        public int Type { get; set; }
        [Column("path")]
        public string Path { get; set; }
        public Company Company { get; set; }
    }

    [Table("oked")]
    public class Oked
    {
        [Key]
        [Column("i")]
        public string Id { get; set; }
        [Column("path")]
        public string Path { get; set; }
    }
    [Table("company")]
    public class ParsedCompany
    {
        [Key]
        [Column("iin_bin")]
        public long Bin { get; set; }
    }
}