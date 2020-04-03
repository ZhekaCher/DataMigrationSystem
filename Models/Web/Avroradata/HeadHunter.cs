using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Web.Avroradata
{
    [Table("hh_companies")]
    public class CompanyHh
    {
        [Key][Column("id")] public long Id { get; set; }
        [Column("name")] public string? Name { get; set; }
        [Column("comp_id")] public long CompId { get; set; }
        [Column("region")] public string? Region { get; set; }
        [Column("web_site")] public string? WebSite { get; set; }
        [Column("activities")] public string? Activities { get; set; }
        [Column("description")] public string? Description { get; set; }
        [Column("logo")] public string? Logo { get; set; }
        [Column("verified")] public Boolean? Verified { get; set; }
    }
    [Table("hh_vacancies")]
    public class VacancyHh
    {
        [Key][Column("id")] public long Id { get; set; }
        [Column("vac_title")] public string? VacTitle { get; set; }
        [Column("salary")] public string? Salary { get; set; }
        [Column("region")] public string? Region { get; set; }
        [Column("description")] public string? Description { get; set; }
        [Column("contacts_fio")] public string? ContactsFio { get; set; }
        [Column("create_time")] public string? CreateTime { get; set; }
        [Column("vac_id")] public long? VacId { get; set; }
        [Column("contacts_phone")] public string? ContactsPhone { get; set; }
        [Column("contacts_email")] public string? ContactsEmail { get; set; }
        [Column("relevance_date")] public DateTime? RelevanceDate { get; set; }
        [Column("active")] public Boolean? Active { get; set; }
        [Column("comp_id")] public long? CompId { get; set; }
        [Column("skills")] public string? Skills { get; set; }
        [Column("experience")] public string? Experience { get; set; }
        [Column("employment")] public string? Employment { get; set; }
    }
}