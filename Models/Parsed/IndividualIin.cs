﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMigrationSystem.Models.Parsed
{
    [Table("individuals")]

    public class IndividualIin
    {
        [Key] [Column("iin")] public long? Code { get; set; }
    }
}