using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("kgd_debtors")]
    public class KgdDebtorsDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("region")]
        public string Region { get; set; }
        
        [Column("revenue_authority")]
        public string RevenueAuthority { get; set; }
        
        [Column("code")]
        public long Code { get; set; }
        
        [Column("taxpayer_name")]
        public string TaxpayerName { get; set; }
        
        [Column("economical_activity")]
        public string EconomicalActivity { get; set; }
        
        [Column("iin_biin")]
        public long? IinBiin { get; set; }
        
        [Column("rnn")]
        public long? Rnn { get; set; }
        
        [Column("fullname")]
        public string Fullname { get; set; }
        
        [Column("total_debt")]
        public double? TotalDebt { get; set; }
        
        [Column("main_debt")]
        public double? MainDebt { get; set; }
        
        [Column("foamy")]
        public double? Foamy { get; set; }
        
        [Column("fine")]
        public double? Fine { get; set; }
        
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
    
    [Table("kgd_debtors_agents")]
    public class KgdDebtorsAgentsDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("region")]
        public string Region { get; set; }
        
        [Column("revenue_authority")]
        public string RevenueAuthority { get; set; }
        
        [Column("code")]
        public long? Code { get; set; }
        
        [Column("agent_name")]
        public string AgentName { get; set; }

        [Column("iin_biin")]
        public long? IinBiin { get; set; }
        
        [Column("rnn")]
        public long? Rnn { get; set; }
        
        [Column("fullname")]
        public string Fullname { get; set; }
        
        [Column("debt_sum")]
        public double? DebtSum { get; set; }
        
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
    [Table("kgd_debtors_customers")]
    public class KgdDebtorsCustomersDto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        
        [Column("region")]
        public string Region { get; set; }
        
        [Column("revenue_authority")]
        public string RevenueAuthority { get; set; }

        [Column("iin_biin")]
        public long? IinBiin { get; set; }

        [Column("fullname")]
        public string Fullname { get; set; }
        
        [Column("debt_sum")]
        public double? DebtSum { get; set; }
        
        [Column("relevance_date")]
        public DateTime RelevanceDate { get; set; }
    }
}