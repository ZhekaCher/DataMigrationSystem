﻿using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{
    public class WebQamqorFinesContext : WebContext
    {
        public DbSet<QamqorFines> QamqorFineses { get; set; }
    }
}