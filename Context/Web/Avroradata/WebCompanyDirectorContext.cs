﻿using DataMigrationSystem.Models.Web.Avroradata;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationSystem.Context.Web.Avroradata
{

    /// @author Yevgeniy Cherdantsev
    /// @date 29.02.2020 15:29:37
    /// @version 1.0
    /// <summary>
    /// INPUT
    /// </summary>
    public class WebCompanyDirectorContext : WebAvroradataContext
    {
        public DbSet<CompanyDirector> CompanyDirectors { get; set; }    
        public DbSet<DataSource> DataSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyDirector>().HasKey(x => new {x.CompanyBin, x.DirectorIin});
        }
    }
}