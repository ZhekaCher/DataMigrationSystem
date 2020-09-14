using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("nadloc_lots")]
    public class LotNadlocDto
    {
        [Column("id")] public long? Id{get; set;}
        [Column("scp_description")] public string ScpDescription{get; set;}
        [Column("scp_code")] public string ScpCode{get; set;}
        [Column("tru_description")] public string TruDescription{get; set;}
        [Column("unit")] public string Unit{get; set;}
        [Column("quantity")] public double? Quantity{get; set;}
        [Column("full_price")] public double? FullPrice{get; set;}
        [Column("delivery_time")] public string DeliveryTime{get; set;}
        [Column("delivery_place")] public string DeliveryPlace{get; set;}
        [Column("contracts")] public string Contracts{get; set;}
        [Column("local_requirements")] public string LocalRequirements{get; set;}
        [Column("required_contract_term")] public string RequiredContractTerm{get; set;}
        [Column("tech_doc_link")] public string TechDocLink{get; set;}
        [Column("tech_doc_name")] public string TechDocName{get; set;}
        [Column("tech_doc_path")] public string TechDocPath{get; set;}
        [Column("contract_doc_link")] public string ContractDocLink{get; set;}
        [Column("contract_doc_name")] public string ContractDocName{get; set;}
        [Column("contract_doc_path")] public string ContractDocPath{get; set;}
        [Column("lot_number")] public long? LotNumber{get; set;}
        [Column("tender_id")] public string TenderId{get; set;}
        [Column("relevance_date")] public DateTime RelevanceDate{get; set;}
		
    }
}