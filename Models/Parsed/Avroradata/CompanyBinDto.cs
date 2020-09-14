using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed.Avroradata
{
    [Table("companies")]
    public class CompanyBinDto
    {
        [Key] 
        [Column("biin")] 
        public long? Code {get; set;}
        [NotMapped]
        public List<EnforcementDebtDto> EnforcementDebtDtos { get; set; }
        [NotMapped]
        public List<LocalCertificatesDto> LocalCertificatesDto { get; set; }
        [NotMapped]
        public List<IndustrialCertificatesDto> IndustrialCertificatesDtos { get; set; }
        [NotMapped]
        public List<ExportCertificatesDto> ExportCertificatesDtos { get; set; }
        [NotMapped]
        public List<UnscrupulousSuppliersDto> UnscrupulousSuppliersDtos { get; set; }
        
    }
}