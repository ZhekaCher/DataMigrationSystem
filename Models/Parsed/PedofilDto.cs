using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("pedofils")]
    public class PedofilDto
    { 
        [Key] 
        [Column("id")]
        public long Id { get; set; }
        [Column("last_name")]
        public string LastName { get;set; }
        [Column("first_name")]
        public string FirsName { get;set; }
        [Column("middle_name")]
        public string MiddleName { get;set; }
        [Column("birthday")]
        public DateTime Birthday{ get;set; }
        [Column("iin")]
        public long Iin { get;set; }
        [Column("gender")]
        public string Gender { get;set; }
        [Column("court")]
        public string Court { get;set; }
        [Column("court_date")]
        public string CourtDate{ get;set; }
        [Column("crime_article")]
        public string CrimeArticle { get;set; }
        [Column("judgement")]
        public string Judgement { get;set; }
        [Column("jail_release_date")]
        public DateTime JailReleaseDate { get;set; }
        [Column("address")]
        public string Address { get;set; }
    }
}