using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("custom_union_declarations")]
    public class CustomUnionDeclarationsDto
    {
        [Key] [Column("id")]
        public long Id { get; set; }

        [Column("reg_num")] 
        public string RegNum { get; set; }

        [Column("validity_from")] 
        public DateTime? ValFrom { get; set; }

        [Column("validity_until")] 
        public DateTime? ValUntil { get; set; }

        [Column("declarants_name")] 
        public string DeclarantsName { get; set; }

        [Column("declarants_iin_biin")] 
        public long? DeclarantsIinBiin { get; set; }

        [Column("declarants_address")] 
        public string DeclarantsAddress { get; set; }

        [Column("manufacturers_name")] 
        public string ManufacturersName { get; set; }

        [Column("manufacturers_address")] 
        public string ManufacturersAdress { get; set; }

        [Column("cert_org_name")] 
        public string CertOrgName { get; set; }

        [Column("cert_org_address")] 
        public string CertOrgAddress { get; set; }

        [Column("cert_org_header")] 
        public string CertOrgHeader { get; set; }

        [Column("product_name")] 
        public string ProductName { get; set; }

        [Column("identificate_inform")]
        public string IdentificateInform { get; set; }

        [Column("tn_code")]
        public string TnCode { get; set; }

        [Column("npa")]
        public string Npa { get; set; }

        [Column("evidance_doc")]
        public string EvidanceDoc { get; set; }

        [Column("extension_inform")]
        public string ExtensionInform { get; set; }

        [Column("details")] 
        public string Details { get; set; }

        [Column("relevance_date")] public DateTime RelevanceDate { get; set; } = DateTime.Now;
        
        public List<CustomUnionDeclarationsAdDto> CustomUnionDeclarationsAdDto { get; set; }
    }

    [Table("custom_union_declarations_ad")]
    public class CustomUnionDeclarationsAdDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("ct_rk")]
        public string CtRk { get; set; }
        
        [Column("tn_vad")]
        public string TnVad { get; set; }
        
        [Column("declaration")]
        public string Declarations { get; set; }
        
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
    }
}