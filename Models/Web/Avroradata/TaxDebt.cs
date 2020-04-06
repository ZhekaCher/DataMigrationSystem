using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("tax_debt")]
    public class TaxDebt
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("bin")]
        public long IinBin { get; set; }
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; } = DateTime.Now;
        [Column("total")]
        public double Total { get; set; }
        [Column("pension_contribution")]
        public double PensionContribution { get; set; }
        [Column("social_contribution")]
        public double SocialContribution{ get; set; }
        [Column("social_health_insurance")]
        public double SocialHealthInsurance{ get; set; }
        public List<TaxDebtOrg> TaxDebtOrgs { get; set; }
    }
    
    [Table("tax_debt_org")]
    public class TaxDebtOrg
    {
        [Column("char_code")] 
        public long CharCode { get; set; }
        [Column("total")] 
        public double Total { get; set; }
        [Column("total_tax")] 
        public double TotalTax { get; set; }
        [Column("pension_contribution")] 
        public double PensionContribution { get; set; }
        [Column("social_contribution")] 
        public double SocialContribution { get; set; }
        [Column("social_health_insurance")] 
        public double SocialHealthInsurance { get; set; }
        [Column("bin")] 
        public long IinBin { get; set; }
        public List<TaxDebtPayer> TaxDebtPayers { get; set; }
        public TaxDebt TaxDebt { get; set; }
    }
    
    [Table("tax_debt_payer")]
    public class TaxDebtPayer
    {
        [Column("bin")] 
        public long IinBin { get; set; }
        [Column("total")] 
        public double Total { get; set; }
        [Column("char_code")] 
        public long CharCode { get; set; }
        [Column("head_bin")] 
        public long HeadIinBin { get; set; }
        public List<TaxDebtBcc> TaxDebtBccs { get; set; }
        public TaxDebtOrg TaxDebtOrg { get; set; }
    }
    
    
    [Table("tax_debt_bcc")]
    public class TaxDebtBcc
    {
        [Column("bcc")]
        public long Bcc { get; set; }
        [Column("tax")]
        public double Tax { get; set; }
        [Column("poena")]
        public double Poena { get; set; }
        [Column("fine")]
        public double Fine { get; set; }
        [Column("total")]
        public double Total { get; set; }
        [Column("char_code")]
        public long CharCode { get; set; }
        [Column("bin")]
        public long IinBin { get; set; }
        public TaxDebtPayer TaxDebtPayer { get; set; }
    }
    
    [Table("tax_debt_org_name")]
    public class TaxDebtOrgName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("char_code")]
        public long CharCode { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("name_kk")]
        public string NameKk { get; set; }
    }
    
    [Table("tax_debt_bcc_name")]
    public class TaxDebtBccName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("bcc")]
        public long Bcc { get; set; }
        [Column("name_ru")]
        public string NameRu { get; set; }
        [Column("name_kk")]
        public string NameKk { get; set; }
    }
}