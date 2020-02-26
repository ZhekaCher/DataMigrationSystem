using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomeNamespace
{
    [Table("company_bin")]
    public class CompanyBinDto
    {
        [Key] [Column("code")] public long? Code{get; set;}
		
    }
}