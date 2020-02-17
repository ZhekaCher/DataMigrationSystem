using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models
{    
    public class BaseReferenceEntity
    {
        [Column("id")]
        public int Id { get; set;}
        [Column("name")]
        public string Name { get; set; }
    }
}