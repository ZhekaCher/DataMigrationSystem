using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("companies")]
    public class CompanyBinDto
    {
        [Key] 
        [Column("biin")] 
        public long? Code {get; set;}
        [NotMapped]
        public List<EnforcementDebtDto> EnforcementDebtDtos { get; set; } 
    }
}