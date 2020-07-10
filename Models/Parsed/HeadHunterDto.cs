using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("hh_companies")]
    public class CompanyHhDto
    {
        [Key] [Column("id")] public long Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("comp_id")] public long CompId { get; set; }
        [Column("region")] public string Region { get; set; }
        [Column("web_site")] public string WebSite { get; set; }
        [Column("activities")] public string Activities { get; set; }
        [Column("description")] public string Description { get; set; }
        [Column("logo")] public string Logo { get; set; }
        [Column("verified")] public Boolean? Verified { get; set; }
    }
    [Table("hh_vacancies")]
    public class VacancyHhDto
    {
        [Key][Column("id")] public long Id { get; set; }
        [Column("vac_title")] public string VacTitle { get; set; }
        [Column("salary")] public string Salary { get; set; }
        [Column("region")] public string Region { get; set; }
        [Column("description")] public string Description { get; set; }
        [Column("contacts_fio")] public string ContactsFio { get; set; }
        [Column("create_time")] public string CreateTime { get; set; }
        [Column("vac_id")] public long? VacId { get; set; }
        [Column("contacts_phone")] public string ContactsPhone { get; set; }
        [Column("contacts_email")] public string ContactsEmail { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
        [Column("active")] public Boolean? Active { get; set; }
        [Column("comp_id")] public long? CompId { get; set; }
        [Column("skills")] public string Skills { get; set; }
        [Column("experience")] public string Experience { get; set; }
        [Column("employment")] public string Employment { get; set; }
        [Column("source")] public string Source { get; set; }

        
    }

    [Table("hhcompbins")]
    public class CompBinhhDto
    {
        [Key][Column("id")] public long Id { get; set; }
        [Column("comp_id")] public long CompId { get; set; }
        [Column("code_bin")] public long CodeBin { get; set; }
        [Column("name_ru")] public string NameRu { get; set; }
        [Column("comp_name")] public string CompName { get; set; }
    }

    [Table("hh_resume")]
    public class HhResumeDto
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] public long Id { get; set; }
        [Column("resume_id")] public long ResumeId { get; set; }
        [Column("gender")] public string Gender { get; set; }
        [Column("age")] public string Age { get; set; }
        [Column("address")] public string Address { get; set; }
        [Column("job")] public string Job { get; set; }
        [Column("salary")] public string Salary { get; set; }
        [Column("general_exp")] public string GeneralExp { get; set; }
        [Column("work_for")] public string WorkFor { get; set; }
        [Column("skills")] public string Skills { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
        [Column("url")] public string Url { get; set; }
        [Column("update_date")] public DateTime UpdateDate { get; set; }
    }
    [Table("hh_resume_bin")]
    public class HhResumeBinDto
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")] public long Id { get; set; }
        [Column("resume_id")] public long ResumeId { get; set; }
        [Column("bin")] public long Bin { get; set; }
        [Column("work_place")] public string WorkPlace { get; set; }
        [Column("work_interval")] public string WorkInterval { get; set; }
        [Column("work_duration")] public string WorkDuration { get; set; }
        [Column("work_pos")] public string WorkPos { get; set; }
        [Column("relevance_date")] public DateTime RelevanceDate { get; set; }
        [Column("update_date")] public DateTime UpdateDate { get; set; }
        [Column("start_work")] public DateTime StartWork { get; set; }
    }
    
    
}