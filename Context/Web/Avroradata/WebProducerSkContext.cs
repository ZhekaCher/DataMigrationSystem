﻿using DataMigrationSystem.Models.Parsed;
using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebProducerSkContext: WebAvroradataContext
    {
        public DbSet<ProducersSk> ProducerSks { get; set; }
        public DbSet<ProducerProducts> ProducerProductses { get; set; }
    }
}